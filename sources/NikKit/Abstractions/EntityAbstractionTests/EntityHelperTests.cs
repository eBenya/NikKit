using NikKit.EntityAbstraction;
using NikKit.EntityAbstractionTests.ExampleModels;

namespace NikKit.EntityAbstractionTests;

public class EntityHelpersTests
{
    private readonly DateTime _now = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero).UtcDateTime;

    private Country CreateCountry(string isoCode, string? description = null, int regionCount = 0, int citiesPerRegion = 0)
    {
        var countryId = Guid.NewGuid();

        var country = new Country
        {
            Id = countryId,
            Created = _now.AddDays(-1),
            Updated = _now.AddDays(-1),
            State = EntityState.Added,
            IsoCode = isoCode,
            Description = description,
            Regions = []
        };

        for (var i = 0; i < regionCount; i++)
        {
            country.Regions.Add(CreateRegion(countryId, $"R{i}", null, citiesPerRegion));
        }

        return country;
    }

    private Region CreateRegion(Guid countryId, string regionCode, string? descriptions = null, int cityCount = 0)
    {
        var regionId = Guid.NewGuid();

        var region = new Region
        {
            Id = regionId,
            CountryId = countryId,
            Code = regionCode,
            Created = _now.AddDays(-1),
            Updated = _now.AddDays(-1),
            State = EntityState.Added,
            Description = descriptions ?? $"Region {regionCode} description",
            Cities = []
        };

        for (var i = 0; i < cityCount; i++)
        {
            region.Cities.Add(CreateCity(countryId, regionId, $"City-{regionCode}-{i}", $"{i}000"));
        }

        return region;
    }

    private City CreateCity(Guid countryId, Guid regionId, string name, string postCode)
    {
        return new City
        {
            Id = Guid.NewGuid(),
            CountryId = countryId,
            RegionId = regionId,
            Name = name,
            PostCode = postCode,
            Created = _now.AddDays(-1),
            Updated = _now.AddDays(-1),
            State = EntityState.Added,
            Description = $"City {name} in region {regionId}"
        };
    }

    [Fact]
    public void Merge_NewIdentity_ShouldBeAddedToCollection()
    {
        // Arrange
        var current = new List<Country> { CreateCountry("DE", "Exist") };
        var incoming = new List<Country> { CreateCountry("DE", "Update"), CreateCountry("IT", "New") };

        // Act
        current.Merge<Country, CountryIdentity>(incoming, _now);

        // Assert
        Assert.Equal(2, current.Count);
        Assert.Contains(current, e => e is { IsoCode: "IT", State: EntityState.Added });
    }

    [Fact]
    public void Merge_ExistingEntity_ShouldBeUpdated()
    {
        // Arrange
        var existing = CreateCountry("US", "Old Data");
        var current = new List<Country> { existing };

        var updatedEntity = CreateCountry("US", "New Data") with{Updated = _now};
        var incoming = new List<Country> { updatedEntity };

        // Act
        current.Merge<Country, CountryIdentity>(incoming, _now);

        // Assert
        Assert.Equal(_now, existing.Updated);
        Assert.Equal(EntityState.Updated, existing.State);
    }

    [Fact]
    public void Merge_TechnicalIdShouldRemainUnchanged_WhenUpdating()
    {
        // Arrange
        var originalGuid = Guid.NewGuid();
        var existing = CreateCountry("CH", "Old Data") with { Id = originalGuid };
        var current = new List<Country> { existing };

        var incomingEntity = CreateCountry("CH", "New Data");
        var incoming = new List<Country> { incomingEntity };

        // Act
        current.Merge<Country, CountryIdentity>(incoming, _now);

        // Assert
        Assert.Equal(originalGuid, current[0].Id);
        Assert.Equal("New Data", current[0].Description);
        Assert.Equal(EntityState.Updated, current[0].State);
        Assert.NotEqual(incomingEntity.Id, current[0].Id);
    }

    [Fact]
    public void Merge_MissingInIncoming_ShouldSetStatusToDeleted()
    {
        // Arrange
        var existing = CreateCountry("FR", "To be deactivated");
        var current = new List<Country> { existing };
        var incoming = new List<Country>();

        // Act
        current.Merge<Country, CountryIdentity>(incoming, _now);

        // Assert
        Assert.Equal(_now, existing.Updated);
        Assert.Equal(EntityState.Deleted, existing.State);
    }

    [Fact]
    public void Merge_WhenCurrentAlreadyHasDeletedEntity_ShouldReactivateIt()
    {
        // Arrange
        var inactive = CreateCountry("NL", "Dead");
        inactive.State = EntityState.Deleted;
        var current = new List<Country> { inactive };

        var active = CreateCountry("NL", "Alive");
        var incoming = new List<Country> { active };

        // Act
        current.Merge<Country, CountryIdentity>(incoming, _now);

        // Assert
        Assert.Equal("Alive", inactive.Description);
    }

    [Fact]
    public void Merge_DuplicateIdentitiesInIncoming_ShouldTakeFirstAndNotFail()
    {
        // Arrange
        var current = new List<Country>();
        var incoming = new List<Country> { CreateCountry("ES", "First"), CreateCountry("ES", "Second") };

        // Act
        current.Merge<Country, CountryIdentity>(incoming, _now);

        // Assert
        Assert.Single(current);
        Assert.Equal("First", current[0].Description);
    }

    [Fact]
    public void Merge_CollectionsAreNull_ShouldNotThrowException()
    {
        // Arrange
        List<Country>? current = null;
        var incoming = new List<Country> { CreateCountry("CA", "Data") };

        // Act & Assert
        var exception = Record.Exception(() => current.Merge<Country, CountryIdentity>(incoming, _now));
        Assert.Null(exception);
    }

    [Fact]
    public void Merge_ComplexScenario_HandlesAllCases()
    {
        // Arrange
        var currentEntities = new List<Country>
        {
            CreateCountry("US", "Entity1Old"),
            CreateCountry("GB", "Entity2Old"),
            CreateCountry("FR", "Entity3Old")
        };
        var newEntities = new List<Country>
        {
            CreateCountry("US", "Entity1New"),
            CreateCountry("DE", "Entity4New"),
            CreateCountry("JP", "Entity5New")
        };

        // Act
        currentEntities.Merge<Country, CountryIdentity>(newEntities, _now);

        // Assert
        Assert.Equal(5, currentEntities.Count);

        Assert.Equal("Entity1New", currentEntities.First(e => e.IsoCode == "US").Description);
        Assert.Equal(EntityState.Updated, currentEntities.First(e => e.IsoCode == "US").State);
        Assert.Equal(EntityState.Deleted, currentEntities.First(e => e.IsoCode == "GB").State);
        Assert.Equal(EntityState.Deleted, currentEntities.First(e => e.IsoCode == "FR").State);
        Assert.Equal("Entity4New", currentEntities.First(e => e.IsoCode == "DE").Description);
        Assert.Equal("Entity5New", currentEntities.First(e => e.IsoCode == "JP").Description);
    }
    
    // With updateNested flag:
    [Fact]
    public void Update_WithUpdateNestedTrue_ShouldMergeRegions()
    {
        // Arrange
        var existingCountry = CreateCountry("US");
        existingCountry = existingCountry with { Regions = [CreateRegion(existingCountry.Id, "NY", "Old NY")] };

        var updatedCountry = CreateCountry("US");
        updatedCountry = updatedCountry with
        {
            Description = "Updated Country",
            Regions =
            [
                CreateRegion(updatedCountry.Id, "NY", "New NY"),
                CreateRegion(updatedCountry.Id, "CA", "New CA")
            ]
        };

        // Act
        existingCountry.Update(updatedCountry, _now, updateNested: true);

        // Assert
        Assert.Multiple(
            // Проверка обновления основной сущности
            () => Assert.Equal("Updated Country", existingCountry.Description),
            
            // Проверка слияния коллекции Regions
            () => Assert.Equal(2, existingCountry.Regions.Count),
            () => Assert.Equal("New NY", existingCountry.Regions.First(r => r.Code == "NY").Description),
            () => Assert.Contains(existingCountry.Regions, r => r.Code == "CA"),
            
            // Проверка mapBeforeAdd (Id страны проставился регионам)
            () => Assert.All(existingCountry.Regions, r => Assert.Equal(existingCountry.Id, r.CountryId)) 
        );
    }

    [Fact]
    public void Update_MissingRegionInIncoming_ShouldMarkRegionAsDeleted()
    {
        // Arrange
        var country = CreateCountry("FR");
        country = country with { Regions = [CreateRegion(country.Id, "IDF", "Paris Region")] };

        var incoming = country with { Regions = [] };

        // Act
        country.Update(incoming, _now, updateNested: true);

        // Assert
        var region = country.Regions.First();
        Assert.Equal(EntityState.Deleted, region.State);
    }
}