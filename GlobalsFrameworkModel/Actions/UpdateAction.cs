using GlobalsFramework.Access;
using GlobalsFramework.Linq;

namespace GlobalsFramework.Actions
{
    internal class UpdateAction<TEntity> : EntityAction<TEntity> where TEntity : class
    {
        public UpdateAction(TEntity entity, DbSet<TEntity> dbSet) : base(entity, dbSet) { }

        public override void Execute()
        {
            DatabaseManager.UpdateEntity(Entity, DbSet.CreateNodeReference(), DbSet.Context.GetConnection());
        }
    }
}
