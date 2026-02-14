using EntityAbstraction.Abstractions;

namespace EntityAbstraction;

public static class EntityHelpers
{
    public static void Merge<TEntity, TIdentity>(
        this ICollection<TEntity>? currentEntities,
        IEnumerable<TEntity>? newEntities,
        DateTime updated,
        Func<TEntity, TEntity>? mapBeforeAdd = null,
        bool updateNested = false)
        where TEntity : BaseEntity<TIdentity>, IUpdatableEntity<TEntity>
        where TIdentity : IEntityIdentity
    {
        if (currentEntities == null) { return; }
        newEntities ??= [];

        // Создаем быстрый поиск для новой коллекции по Identity
        var newEntitiesDict = newEntities
                              .GroupBy(e => e.Identity)
                              .ToDictionary(g => g.Key, g => g.First());

        // Обновляем существующие или помечаем как удаленные (OdaStatus.NotActive)
        foreach (var existingEntity in currentEntities)
        {
            if (newEntitiesDict.TryGetValue(existingEntity.Identity, out var match))
            {
                // Нашли совпадение по бизнес-ключу.
                // Обновляем данные, при этом существующий Id (Guid) остается прежним.
                existingEntity.Update(match, updated, updateNested);

                // Удаляем из словаря, чтобы пометить как "обработанный"
                newEntitiesDict.Remove(existingEntity.Identity);
            }
            else
            {
                // В новых данных этого бизнес-ключа нет -> Мягкое удаление
                existingEntity.Update(null, updated, updateNested);
            }
        }

        // Добавляем те новые сущности, которых вообще не было в базе по Identity
        foreach (var newEntry in newEntitiesDict.Values)
        {
            var entityToAdd = mapBeforeAdd != null ? mapBeforeAdd(newEntry) : newEntry;
            currentEntities.Add(entityToAdd with { Id = default });
        }
    }
}
