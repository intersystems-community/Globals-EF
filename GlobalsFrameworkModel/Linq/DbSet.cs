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
    /// <summary>
    ///A <see cref="T:GlobalsFramework.Linq.DbSet`1"/> represents an entity that is used for create, read, update, and delete operations to the database.
    /// </summary>
    /// <typeparam name="TEntity">The type that defines the set.</typeparam>
    public sealed class DbSet<TEntity> : IDbSet<TEntity>, IOrderedQueryable<TEntity>, IQueryProvider where TEntity : class
    {
        private readonly string _dbSetName;

        private DbSet(DataContext context)
        {
            Context = context;

            _dbSetName = typeof(TEntity).Name;
            var dbSetAttribute = EntityTypeDescriptor.GetTypeDescription(typeof(TEntity)).DbSetAttribute;
            if (dbSetAttribute != null)
                _dbSetName = dbSetAttribute.Name ?? _dbSetName;
        }

        /// <summary>
        /// Gets the <see cref="T:GlobalsFramework.DataContext"/> that has been used to retrieve this <see cref="T:GlobalsFramework.Linq.DbSet`1"/>.
        /// </summary>
        public DataContext Context { get; private set; }

        Expression IQueryable.Expression
        {
            get { return Expression.Constant(this); }
        }

        Type IQueryable.ElementType
        {
            get { return typeof(TEntity); }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return this; }
        }

        /// <summary>
        /// Adds an entity in a pending insert state.
        /// </summary>
        /// <param name="entity">The entity to be inserted.</param>
        public void InsertOnSubmit(TEntity entity)
        {
            Context.RegisterAction(new InsertAction<TEntity>(entity, this));
        }

        /// <summary>
        /// Puts all entities from the collection into a pending insert state.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        public void InsertAllOnSubmit(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Context.RegisterAction(new InsertAction<TEntity>(entity, this));
            }
        }

        /// <summary>
        /// Adds an entity in a pending update state.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        public void UpdateOnSubmit(TEntity entity)
        {
            Context.RegisterAction(new UpdateAction<TEntity>(entity, this));
        }

        /// <summary>
        /// Puts all entities from the collection into a pending update state.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        public void UpdateAllOnSubmit(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Context.RegisterAction(new UpdateAction<TEntity>(entity, this));
            }
        }

        /// <summary>
        /// Adds an entity in a pending delete state.
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        public void DeleteOnSubmit(TEntity entity)
        {
            Context.RegisterAction(new DeleteAction<TEntity>(entity, this));
        }

        /// <summary>
        /// Puts all entities from the collection into a pending delete state.
        /// </summary>
        /// <param name="entities">The entities to delete.</param>
        public void DeleteAllOnSubmit(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Context.RegisterAction(new DeleteAction<TEntity>(entity, this));
            }
        }

        internal NodeReference CreateNodeReference()
        {
            return Context.CreateNodeReference(_dbSetName);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            return new EntityQuery<TEntity>(this, expression);
        }

        IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
        {
            return new EntityQuery<TElement>(this, expression);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return QueryProcessingHelper.ProcessQueries(CreateNodeReference(), Context, expression);
        }

        TResult IQueryProvider.Execute<TResult>(Expression expression)
        {
            return (TResult) ((IQueryProvider) this).Execute(expression);
        }

        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
        {
            return this.Select(p => p).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TEntity>)this).GetEnumerator();
        }
    }
}
