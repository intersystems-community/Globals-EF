using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GlobalsFramework.Actions;
using GlobalsFramework.Linq.Helpers;
using GlobalsFramework.Utils.TypeDescription;
using InterSystems.Globals;

namespace GlobalsFramework.Linq
{
    public sealed class DbSet<TEntity> : IDbSet<TEntity>, IOrderedQueryable<TEntity>, IQueryProvider where TEntity : class
    {
        private readonly string _dbSetName;

        private DbSet(DataContext context)
        {
            Context = context;
            Expression = Expression.Constant(this);
            Provider = this;

            _dbSetName = typeof(TEntity).Name;
            var dbSetAttribute = EntityTypeDescriptor.GetTypeDescription(typeof(TEntity)).DbSetAttribute;
            if (dbSetAttribute != null)
                _dbSetName = dbSetAttribute.Name ?? _dbSetName;
        }

        public DataContext Context { get; private set; }

        public Expression Expression { get; private set; }

        public Type ElementType
        {
            get
            {
                return typeof(TEntity);
            }
        }

        public IQueryProvider Provider { get; private set; }

        public void InsertOnSubmit(TEntity entity)
        {
            Context.RegisterAction(new InsertAction<TEntity>(entity, this));
        }

        public void InsertAllOnSubmit(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Context.RegisterAction(new InsertAction<TEntity>(entity, this));
            }
        }

        public void UpdateOnSubmit(TEntity entity)
        {
            Context.RegisterAction(new UpdateAction<TEntity>(entity, this));
        }

        public void UpdateAllOnSubmit(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Context.RegisterAction(new UpdateAction<TEntity>(entity, this));
            }
        }

        public void DeleteOnSubmit(TEntity entity)
        {
            Context.RegisterAction(new DeleteAction<TEntity>(entity, this));
        }

        public void DeleteAllOnSubmit(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Context.RegisterAction(new DeleteAction<TEntity>(entity, this));
            }
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new EntityQuery<TEntity>(this, expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new EntityQuery<TElement>(this, expression);
        }

        public object Execute(Expression expression)
        {
            return QueryProcessingHelper.ProcessQueries(CreateNodeReference(), Context, expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)Execute(expression);
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return this.Select(p => p).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal NodeReference CreateNodeReference()
        {
            return Context.GetConnection().CreateNodeReference(_dbSetName);
        }
    }
}
