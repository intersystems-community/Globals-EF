using GlobalsDbFrameworkModel.Model.Access;

namespace GlobalsDbFrameworkModel.Model.Linq.EntityCommands
{
    internal class UpdateEntityCommand<TEntity> : EntityCommand<TEntity>
    {
        public UpdateEntityCommand(TEntity entity, DataContext context) : base(entity, context) { }

        protected override void Execute(TEntity entity, DataContext context)
        {
            var connection = context.GetConnection();
            DatabaseManager.UpdateEntity(entity, connection.CreateNodeReference(), connection);
        }
    }
}
