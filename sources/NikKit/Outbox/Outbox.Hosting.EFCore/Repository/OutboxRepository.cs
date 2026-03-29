using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NikKit.Outbox.Abstraction;

namespace NikKit.Outbox.Hosting.EFCore.Repository;

public class OutboxRepository : IOutboxRepository
{
    private readonly DbContext _dbContext;
    private readonly IOptions<OutboxOptions> _outboxOptions;

    public OutboxRepository(DbContext dbContext, IOptions<OutboxOptions> outboxOptions)
    {
        _dbContext = dbContext;
        _outboxOptions = outboxOptions;
        
        CleanupPolicy = _outboxOptions.Value.CleanupPolicy;
        UseExternalUnitOfWork = _outboxOptions.Value.UseExternalUnitOfWork;
    }

    public string CleanupPolicy { get; }
    public bool UseExternalUnitOfWork { get; }
    
    public virtual async Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize, CancellationToken ctk = default)
    {
        throw new NotImplementedException();
    }

    public virtual async Task MarkAsCompletedAsync(IEnumerable<Guid> ids, CancellationToken ctk)
    {
        await _dbContext.Set<OutboxMessage>().Where(m => ids.Contains(m.Id)).ExecuteDeleteAsync(ctk);
    }

    public virtual async Task MarkAsFailedAsync(IEnumerable<OutboxFailureUpdate> failed, CancellationToken ctk)
    {
        throw new NotImplementedException();
    }
}