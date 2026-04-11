namespace NikKit.EntityAbstraction.Abstractions;

public interface IUpdatableEntity<TEntity>
{
    void Update(TEntity? newEntity, DateTime? updatedTime, bool updateNested = false);
    TEntity MatchNestedForAdd(bool updateNested = false);
}
