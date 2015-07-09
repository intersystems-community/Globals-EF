using GlobalsFramework.Access;
using GlobalsFramework.Linq;

namespace GlobalsFramework.Actions
{
    internal class DeleteAction<TEntity> : EntityAction<TEntity> where TEntity : class
    {
        public DeleteAction(TEntity entity, DbSet<TEntity> dbSet) : base(entity, dbSet) { }

        public override void Execute()
        {
            DatabaseManager.DeleteEntity(Entity, DbSet.CreateNodeReference(), DbSet.Context.GetConnection());
        }
    }
}
