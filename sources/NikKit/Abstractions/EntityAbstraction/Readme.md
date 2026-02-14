# EntityAbstraction & Sync Toolkit

A library for building domain models with support for automatic synchronization of nested hierarchies and soft deletion. It is perfectly suited for integration scenarios where incoming DTOs must be mapped against existing database entities.

---

## Key Features

* **Identity-driven Sync:** Entity matching is performed using **business keys** (e.g., ISO country codes or document numbers) rather than technical GUIDs.
* **Deep Merge:** Automatic updates of nested collections (to any depth) while maintaining referential integrity.
* **State Management:** Automatic tracking of entity states (`Added`, `Updated`, `Deleted`) during data merging.
* **Immutable Safety:** Base identities are implemented as `record` types, ensuring safety when handling identifiers.

---

## Model Architecture

The architecture has next abstractions:

1.  **Identity:** A `record` describing a unique business key.
2.  **BaseEntity:** A base class containing technical fields (`Id`, `State`, `Timestamps`).
3.  **IUpdatableEntity:** An interface defining the logic for updating fields and nested entities.

---

## Usage Examples

### 1. Defining Entities (Country -> Region)

```csharp
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
        // Base update for State and Updated timestamp
        base.Update(newEntity, newEntity?.Updated ?? updatedTime ?? DateTime.UtcNow);

        if (newEntity is null) return;

        this.Description = newEntity.Description;

        // Recursive synchronization of the Regions collection
        if (updateNested)
        {
            Regions.Merge<Region, RegionIdentity>(
                newEntity.Regions, 
                Updated,
                mapBeforeAdd: r => r with { CountryId = Id }, // Pass the Foreign Key
                updateNested: true);
        }
    }
}
```
### 2. Collection Synchronization (Merge)

The Merge extension method compares the current collection from the database with an incoming data list.

- If an entity is found via its `Identity`, it is `Updated`.
- If it is missing from the DB, it is `Added`.
- If it is missing from the incoming data, it is marked as `Deleted`.

```csharp
var currentCountries = await dbContext.Countries.Include(c => c.Regions).ToListAsync();
var incomingData = await apiClient.GetCountriesAsync();

// Execute merge
currentCountries.Merge<Country, CountryIdentity>(
    incomingData, 
    DateTime.UtcNow, 
    updateNested: true);

await dbContext.SaveChangesAsync();
```
Testing

The project includes an xUnit test suite covering:

- Persistence of technical GUIDs during business data updates.
- Automatic reactivation of deleted entities if they reappear in the incoming data.
- Correct Foreign Key (FK) assignment for new nested objects via mapBeforeAdd.