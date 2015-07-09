using GlobalsFramework.Linq;

namespace GlobalsFramework.Actions
{
    internal abstract class EntityAction<TEntity> : IEntityAction where TEntity : class
    {
        protected readonly TEntity Entity;
        protected readonly DbSet<TEntity> DbSet;

        protected EntityAction(TEntity entity, DbSet<TEntity> dbSet)
        {
            Entity = entity;
            DbSet = dbSet;
        }

        public abstract void Execute();
    }
}
