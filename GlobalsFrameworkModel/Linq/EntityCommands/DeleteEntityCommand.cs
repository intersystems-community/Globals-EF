using GlobalsDbFrameworkModel.Model.Access;

namespace GlobalsDbFrameworkModel.Model.Linq.EntityCommands
{
    internal class DeleteEntityCommand<TEntity> : EntityCommand<TEntity>
    {
        public DeleteEntityCommand(TEntity entity, DataContext context) : base(entity, context) { }

        protected override void Execute(TEntity entity, DataContext context)
        {
            var connection = context.GetConnection();
            DatabaseManager.DeleteEntity(entity, connection.CreateNodeReference(), connection);
        }
    }
}
