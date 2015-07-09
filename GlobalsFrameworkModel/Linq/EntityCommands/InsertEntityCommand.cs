using GlobalsDbFrameworkModel.Model.Access;

namespace GlobalsDbFrameworkModel.Model.Linq.EntityCommands
{
    internal class InsertEntityCommand<TEntity> : EntityCommand<TEntity>
    {
        public InsertEntityCommand(TEntity entity, DataContext context) : base(entity, context) { }

        protected override void Execute(TEntity entity, DataContext context)
        {
            var connection = context.GetConnection();
            DatabaseManager.Insert(entity, connection.CreateNodeReference(), connection);
        }
    }
}
