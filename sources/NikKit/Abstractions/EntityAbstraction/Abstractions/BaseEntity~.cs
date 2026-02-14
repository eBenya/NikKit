namespace EntityAbstraction.Abstractions;

public abstract record BaseEntity<TIdentity> : BaseEntity
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

    public virtual bool IsEqualTo(BaseEntity<TIdentity>? newEntity)
    {
        if (newEntity is null)
            return false;
        return Identity.Equals(newEntity.Identity);
    }

    protected void Update(BaseEntity<TIdentity>? newEntity, DateTime updatedTime)
    {
        if(newEntity is null)
        {
            State = EntityState.Deleted;
            Updated = updatedTime;
            return;
        }

        State = EntityState.Updated;
        Updated = newEntity.Updated;
    }
}
