using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NikKit.Outbox.Abstraction;
using Quartz;

namespace NikKit.Outbox.Hosting;

public static class OutboxServiceCollectionExtensions
{
    /// <summary>
    /// OutBox worker registration.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddOutboxJob<TRepository>(this IServiceCollection services, IConfiguration configuration)
        where TRepository: class, IOutboxRepository
    {
        var options = configuration.GetSection(OutboxOptions.SectionName).Get<OutboxOptions>() ?? new OutboxOptions();
        
        services.Configure<OutboxOptions>(opt => configuration.GetSection(OutboxOptions.SectionName).Bind(opt));
        services.AddPublishers();
        services.AddQuartzJob(options);
        services.AddScoped<IOutboxRepository, TRepository>();
        return services;
    }

    private static void AddQuartzJob(this IServiceCollection services, OutboxOptions options)
    {
        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("OutboxProcessorJob");

            q.AddJob<OutboxQuartzJob>(opts =>
            {
                opts.WithIdentity(jobKey);
                opts.DisallowConcurrentExecution();
            });

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("OutboxProcessorTrigger")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(options.ProcessingTimeoutSeconds)
                    .RepeatForever()));
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    }


    private static void AddPublishers(this IServiceCollection services)
    {
        var publisherTypes = typeof(IOutboxPublisher).Assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false } && typeof(IOutboxPublisher).IsAssignableFrom(t));

        foreach (var type in publisherTypes)
        {
            services.AddSingleton(typeof(IOutboxPublisher), type);
        }
    }
}