using NikKit.EntityAbstraction;
using NikKit.EntityAbstraction.Abstractions;

namespace NikKit.EntityAbstractionTests.ExampleModels;

public record TestIdentity(int Value): IEntityIdentity
{
    public bool IsValid => Value > 0;
}

public record L1 : BaseEntity<L1, TestIdentity>
{
    public ICollection<L2> Children { get; init; } = [];
    protected override Func<TestIdentity> IdentityCreator() => () => new(1);
    
    internal L2 MapL2(L2 child) => child with { L1Id = Id };

    public override L1 MatchNestedForAdd(bool updateNested = false)
    {
        var preparedChildren = new List<L2>();
        preparedChildren.Merge<L2, TestIdentity>(Children, Updated, MapL2, updateNested);
        
        return this with { Children = preparedChildren };
    }
    
    internal static L1 Create(Guid id)
        => new L1()
        {
            Id = id,
            State =  EntityState.Added,
            Created =  DateTime.UtcNow,
            Updated =  DateTime.UtcNow,
        };

    public override void Update(L1? newEntity, DateTime? updatedTime, bool updateNested = false)
    {
        base.Update(newEntity, updatedTime);
        if(newEntity is null) return;

        if (updateNested)
        {
            Children.Merge<L2, TestIdentity>(newEntity.Children, Updated,
                mapBeforeAdd: MapL2,
                updateNested: updateNested);
        }
    }
}

public record L2 : BaseEntity<L2, TestIdentity>
{
    public Guid L1Id { get; init; }
    public ICollection<L3> Children { get; init; } = [];
    protected override Func<TestIdentity> IdentityCreator() => () => new(2);

    // Маппинг для 3-го уровня: прокидываем ID корня и свой ID
    internal L3 MapL3(L3 child) => child with { L1Id = L1Id, L2Id = Id };

    public override L2 MatchNestedForAdd(bool updateNested = false)
    {
        var preparedChildren = new List<L3>();
        preparedChildren.Merge<L3, TestIdentity>(Children, Updated, MapL3, updateNested);
        
        return this with { Children = preparedChildren };
    }

    internal static L2 Create(Guid id)
        => new L2()
        {
            Id = id,
            State =  EntityState.Added,
            Created =  DateTime.UtcNow,
            Updated =  DateTime.UtcNow,
        };

    public override void Update(L2? newEntity, DateTime? updatedTime, bool updateNested = false)
    {
        base.Update(newEntity, updatedTime);
        if(newEntity is null) return;

        if (updateNested)
        {
            Children.Merge<L3, TestIdentity>(newEntity.Children, Updated,
                mapBeforeAdd: MapL3,
                updateNested: updateNested);
        }
    }
}

public record L3 : BaseEntity<L3, TestIdentity>
{
    public Guid L1Id { get; init; }
    public Guid L2Id { get; init; }
    public ICollection<L4> Children { get; init; } = [];
    protected override Func<TestIdentity> IdentityCreator() => () => new(3);

    // Маппинг для 4-го уровня: прокидываем всю цепочку ID
    internal L4 MapL4(L4 child) => child with { L1Id = L1Id, L2Id = L2Id, L3Id = Id };
    
    public override L3 MatchNestedForAdd(bool updateNested = false)
    {
        var preparedChildren = new List<L4>();
        preparedChildren.Merge<L4, TestIdentity>(Children, Updated, MapL4, updateNested);
        
        return this with { Children = preparedChildren };
    }

    internal static L3 Create(Guid id)
        => new L3()
        {
            Id = id,
            State =  EntityState.Added,
            Created =  DateTime.UtcNow,
            Updated =  DateTime.UtcNow,
        };

    public override void Update(L3? newEntity, DateTime? updatedTime, bool updateNested = false)
    {
        base.Update(newEntity, updatedTime);
        if(newEntity is null) return;

        if (updateNested)
        {
            Children.Merge<L4, TestIdentity>(newEntity.Children, Updated,
                mapBeforeAdd: MapL4,
                updateNested: updateNested);
        }
    }
}

public record L4 : BaseEntity<L4, TestIdentity>
{
    public Guid L1Id { get; init; }
    public Guid L2Id { get; init; }
    public Guid L3Id { get; init; }
    protected override Func<TestIdentity> IdentityCreator() => () => new(4);

    // Нет вложенных коллекций

    internal static L4 Create(Guid id)
        => new L4()
        {
            Id = id,
            State =  EntityState.Added,
            Created =  DateTime.UtcNow,
            Updated =  DateTime.UtcNow,
        };

    public override void Update(L4? newEntity, DateTime? updatedTime, bool updateNested = false)
    {
        base.Update(newEntity, updatedTime);
        if(newEntity is null) return;
    }
}