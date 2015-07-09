using GlobalsFramework.Access;
using GlobalsFramework.Linq;

namespace GlobalsFramework.Actions
{
    internal class InsertAction<TEntity> : EntityAction<TEntity> where TEntity : class
    {
        public InsertAction(TEntity entity, DbSet<TEntity> dbSet) : base(entity, dbSet) { }

        public override void Execute()
        {
            DatabaseManager.InsertEntity(Entity, DbSet.CreateNodeReference(), DbSet.Context.GetConnection());
        }
    }
}
