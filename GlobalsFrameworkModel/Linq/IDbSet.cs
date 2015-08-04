using System.Collections.Generic;

namespace GlobalsFramework.Linq
{
    /// <summary>
    /// An <see cref="T:GlobalsFramework.Linq.IDbSet`1"/> represents an entity that is used for performing create, read, update, and delete operations to the database.
    /// <see cref="T:GlobalsFramework.Linq.DbSet`1"/> is a concrete implementation of <see cref="T:GlobalsFramework.Linq.IDbSet`1"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type that defines the set.</typeparam>
    public interface IDbSet<TEntity> where TEntity : class
    {
        /// <summary>
        /// Adds an entity in a pending insert state.
        /// </summary>
        /// <param name="entity">The entity to be inserted.</param>
        void InsertOnSubmit(TEntity entity);

        /// <summary>
        /// Puts all entities from the collection into a pending insert state.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        void InsertAllOnSubmit(IEnumerable<TEntity> entities);

        /// <summary>
        /// Adds an entity in a pending update state.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        void UpdateOnSubmit(TEntity entity);

        /// <summary>
        /// Puts all entities from the collection into a pending update state.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        void UpdateAllOnSubmit(IEnumerable<TEntity> entities);

        /// <summary>
        /// Adds an entity in a pending delete state.
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        void DeleteOnSubmit(TEntity entity);

        /// <summary>
        /// Puts all entities from the collection into a pending delete state.
        /// </summary>
        /// <param name="entities">The entities to delete.</param>
        void DeleteAllOnSubmit(IEnumerable<TEntity> entities);
    }
}
