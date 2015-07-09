namespace GlobalsDbFrameworkModel.Model.Linq.EntityCommands
{
    internal abstract class EntityCommand<TEntity>
    {
        private readonly TEntity _entity;
        private readonly DataContext _context;

        protected EntityCommand(TEntity entity, DataContext context)
        {
            _entity = entity;
            _context = context;
        }

        public void Execute()
        {
            Execute(_entity, _context);
        }

        protected abstract void Execute(TEntity entity, DataContext context);
    }
}
