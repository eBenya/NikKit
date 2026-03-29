using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NikKit.IEnumerableExtensions;
using NikKit.Outbox.Abstraction;
using NikKit.Outbox.Abstraction.Exceptions;
using Quartz;

namespace NikKit.Outbox.Hosting;

public class OutboxQuartzJob : IJob
{
    private readonly IOptionsMonitor<OutboxOptions> _options;
    private readonly ILogger<OutboxQuartzJob> _logger;
    private readonly IOutboxRepository _repository;
    private readonly IPublisherRegistry _publisherRegistry;

    public OutboxQuartzJob(IOptionsMonitor<OutboxOptions> options,
        ILogger<OutboxQuartzJob> logger, 
        IOutboxRepository repository,
        IPublisherRegistry publisherRegistry)
    {
        _options = options;
        _logger = logger;
        _repository = repository;
        _publisherRegistry = publisherRegistry;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var stoppingToken = context.CancellationToken;
        var options = _options.CurrentValue;

        try
        {
            var batch = await _repository.GetUnprocessedMessagesAsync(options.BatchSize, stoppingToken);

            if (batch.Count == 0)
            {
                _logger.LogInformation("No outbox messages were processed.");
                return;
            }

            var completedIds = new List<Guid>();
            var failures = new List<OutboxFailureUpdate>();

            foreach (var message in batch)
            {
                try
                {
                    // preparing message & search event senders
                    var eventMeta = message.Content.Deserialize<IEvent>() ?? throw new BrokenEventMessage(message);
                    var publishers = _publisherRegistry.GetWriters(message.EventType, eventMeta).AsList();
                    if (publishers.Count != 0)
                    {
                        completedIds.Add(message.Id);
                        continue;
                    }
                    var payLoad = message.Content.RootElement.GetRawText();

                    // publish
                    var tasks = publishers.Select(publisher =>
                        publisher.PublishAsync(payLoad, message.EventType, eventMeta, stoppingToken));

                    await Task.WhenAll(tasks);
                    completedIds.Add(message.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process outbox message {Id}", message.Id);
                    
                    var isDeadLetter = message.Attempts + 1 >= options.MaxRetries;
                    var nextRetryAt = (isDeadLetter ? (int?)null : CalculateNextRetry(message.Attempts, options)) ?? 0;
                    
                    failures.Add(new OutboxFailureUpdate(message.Id, ex.Message, nextRetryAt, isDeadLetter));
                }
            }

            await _repository.MarkAsCompletedAsync(completedIds, stoppingToken);
            await _repository.MarkAsFailedAsync(failures, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Fatal error during Quartz Outbox Job execution");
            throw new JobExecutionException(ex);
        }
    }

    // Exponential jitter delay
    private static int CalculateNextRetry(int currentRetryCount, OutboxOptions options)
    {
        var delayMs = options.BaseDelayMs * (int)Math.Pow(2, currentRetryCount);
        var jitter = Random.Shared.Next(0, options.MaxJitterMs);
        return delayMs + jitter;
    }
}