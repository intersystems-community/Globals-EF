using System.Collections.Generic;

namespace GlobalsFramework.Linq
{
    public interface IDbSet<TEntity> where TEntity : class
    {
        void InsertOnSubmit(TEntity entity);
        void InsertAllOnSubmit(IEnumerable<TEntity> entities);
        void UpdateOnSubmit(TEntity entity);
        void UpdateAllOnSubmit(IEnumerable<TEntity> entities);
        void DeleteOnSubmit(TEntity entity);
        void DeleteAllOnSubmit(IEnumerable<TEntity> entities);
    }
}
