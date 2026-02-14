namespace EntityAbstraction.Abstractions;

public abstract record BaseEntity
{
    public required Guid Id { get; init; } = Guid.Empty;
    public required EntityState State { get; set; } = EntityState.Added;
    public required DateTime Created { get; set; }
    public required DateTime Updated { get; set; }

    private protected BaseEntity() { }
}
