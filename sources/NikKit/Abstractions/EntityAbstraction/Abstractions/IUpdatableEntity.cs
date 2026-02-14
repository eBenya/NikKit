namespace NikKit.EntityAbstraction.Abstractions;

public interface IUpdatableEntity<in TEntity>
{
    void Update(TEntity? newEntity, DateTime? updatedTime, bool updateNested = false);
}
