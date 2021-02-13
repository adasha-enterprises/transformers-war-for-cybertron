using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WarForCybertron.Model;

namespace WarForCybertron.Repository
{
    public interface IWarForCybertronRepository<TEntity, TEntity2> : IEnumerable<TEntity> where TEntity : GuidEntity where TEntity2 : DbContext
    {
        TEntity Find(params object[] keyValues);
        TEntity Find(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null);
        void Add(TEntity entity, bool save = true);
        Task AddAsync(TEntity entity, bool save = true);
        void AddRange(ICollection<TEntity> entities, bool save = true);
        void SaveChanges();
        Task SaveChangesAsync();
        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null);
        IEnumerable<TEntity> List();
        IQueryable<TEntity> Set<T>();
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}
