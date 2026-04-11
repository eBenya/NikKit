namespace NikKit.EntityAbstraction.Abstractions;

public abstract record BaseEntity<TEntity, TIdentity> : BaseEntity, IUpdatableEntity<TEntity>
    where TEntity : BaseEntity<TEntity, TIdentity>
    where TIdentity : IEntityIdentity
{
    public TIdentity Identity
    {
        get
        {
            field ??= IdentityCreator()();
            return field;
        }
    }
    protected abstract Func<TIdentity> IdentityCreator();

    public virtual bool IsEqualTo(BaseEntity<TEntity, TIdentity>? newEntity)
    {
        if (newEntity is null)
            return false;
        return Identity.Equals(newEntity.Identity);
    }

    public virtual void Update(TEntity? newEntity, DateTime? updatedTime, bool updateNested = false)
    {
        if(newEntity is null)
        {
            State = EntityState.Deleted;
            Updated = updatedTime ?? DateTime.Now;
            return;
        }

        State = EntityState.Updated;
        Updated = newEntity.Updated;
    }

    public virtual TEntity MatchNestedForAdd(bool updateNested = false)
    {
        return (TEntity)this;
    }
}
