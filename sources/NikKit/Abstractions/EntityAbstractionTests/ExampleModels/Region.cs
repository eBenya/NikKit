using NikKit.EntityAbstraction;
using NikKit.EntityAbstraction.Abstractions;

namespace NikKit.EntityAbstractionTests.ExampleModels;

// Уровень 3 (Самый глубокий)
public record CityIdentity(string Name, string PostCode) : IEntityIdentity
{
    public bool IsValid => !string.IsNullOrEmpty(Name);
}

public record City : BaseEntity<CityIdentity>, IUpdatableEntity<City>
{
    public required Guid RegionId { get; init; }
    public required Guid? CountryId { get; init; }
    public required string Name { get; init; }
    public required string PostCode { get; init; }
    public string? Description { get; set; }
    
    protected override Func<CityIdentity> IdentityCreator() => () => new CityIdentity(Name, PostCode);
    public void Update(City? newEntity, DateTime? updatedTime, bool updateNested = false)
    {
        
    }
}

// Уровень 2
public record RegionIdentity(string Code) : IEntityIdentity
{
    public bool IsValid => !string.IsNullOrEmpty(Code);
}

public record Region : BaseEntity<RegionIdentity>, IUpdatableEntity<Region>
{
    public required Guid CountryId { get; init; }
    public required string Code { get; init; }
    public required List<City> Cities { get; init; } = new();
    public string? Description { get; set; }
    
    protected override Func<RegionIdentity> IdentityCreator() => () => new RegionIdentity(Code);
    public void Update(Region? newEntity, DateTime? updatedTime, bool updateNested = false)
    {
        base.Update(newEntity, updatedTime ?? DateTime.UtcNow);
        if(newEntity is null)
        {
            return;
        }

        Description = newEntity.Description;

        if (updateNested)
        {
            Cities.Merge<City, CityIdentity>(newEntity.Cities, Updated,
                                             mapBeforeAdd: r => r with { CountryId = CountryId, RegionId = Id },
                                             updateNested: updateNested);
        }
    }
}

// Уровень 1 (Топ-левел)
public record CountryIdentity(string IsoCode) : IEntityIdentity
{
    public bool IsValid => IsoCode.Length == 2;
}

public record Country : BaseEntity<CountryIdentity>, IUpdatableEntity<Country>
{
    public required string IsoCode { get; init; }
    public string? Description { get; set; }
    
    public required ICollection<Region> Regions { get; init; } = [];

    protected override Func<CountryIdentity> IdentityCreator() => () => new CountryIdentity(IsoCode);

    public void Update(Country? newEntity, DateTime? updatedTime, bool updateNested = false)
    {
        base.Update(newEntity, newEntity?.Updated ?? updatedTime ?? DateTime.Now);
        if(newEntity is null)
        {
            return;
        }

        Description = newEntity.Description;

        if (updateNested)
        {
            Regions.Merge<Region, RegionIdentity>(newEntity.Regions, Updated,
                                                  mapBeforeAdd: r => r with { CountryId = Id },
                                                  updateNested: updateNested);
        }
    }
}