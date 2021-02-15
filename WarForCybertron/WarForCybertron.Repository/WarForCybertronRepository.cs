using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WarForCybertron.Model;

namespace WarForCybertron.Repository
{
    public class WarForCybertronRepository<TEntity, TEntity2> : IWarForCybertronRepository<TEntity, TEntity2> where TEntity : GuidEntity where TEntity2 : DbContext
    {
        protected readonly TEntity2 Context;

        public TEntity2 Db
        {
            get => Context;
        }

        public WarForCybertronRepository(TEntity2 context)
        {
            Context = context;
        }

        public TEntity Find(params object[] keyValues)
        {
            return Context.Set<TEntity>().Find(keyValues);
        }

        public TEntity Find(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();

            if (include != null)
            {
                query = include(query);
            }

            return query.Where(predicate).AsNoTracking().FirstOrDefault();
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await Context.SaveChangesAsync();
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();

            if (include != null)
            {
                query = include(query);
            }

            return query.Where(predicate).AsNoTracking();
        }

        public IEnumerable<TEntity> List()
        {
            return Context.Set<TEntity>().ToList();
        }

        public void Add(TEntity entity, bool save = true)
        {
            Context.Set<TEntity>().Add(entity);

            if (save)
            {
                Context.SaveChanges();

                StopTracking(entity);
            }
        }

        public async Task AddAsync(TEntity entity, bool save = true)
        {
            Context.Set<TEntity>().Add(entity);

            if (save)
            {
                await Context.SaveChangesAsync();
            }
        }

        public void AddRange(ICollection<TEntity> entities, bool save = true)
        {
            Context.Set<TEntity>().AddRange(entities);

            if (save)
            {
                Context.SaveChanges();

                entities.ToAsyncEnumerable().ForEachAsync(e =>
                    {
                        StopTracking(e);
                    }
                );
            }
        }

        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
        {
            return Context.Set<TEntity>().AsEnumerable().GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return Context.Set<TEntity>().AsEnumerable().GetEnumerator();
        }

        public IQueryable<TEntity> Set<T>()
        {
            return Context.Set<TEntity>();
        }

        public void Update(TEntity entity)
        {
            var entry = Context.Entry(entity);
            entry.State = EntityState.Modified;
        }

        public void Delete(TEntity entity)
        {
            dynamic entityId = entity.Id;

            if (entity is GuidEntity)
            {
                entityId = (entity as GuidEntity).Id;
            }
            var foundEntity = Context.Set<TEntity>().Find(entityId);

            if (foundEntity != null)
            {
                Context.Set<TEntity>().Remove(entity);
                Context.SaveChanges();
            }
        }

        public async Task DeleteAsync(TEntity entity)
        {
            dynamic entityId = entity.Id;

            if (entity is GuidEntity)
            {
                entityId = (entity as GuidEntity).Id;
            }
            var foundEntity = await Context.Set<TEntity>().FindAsync(entityId);

            if (foundEntity != null)
            {
                Context.Set<TEntity>().Remove(entity);
                await Context.SaveChangesAsync();
            }
        }

        public void StopTracking(TEntity entity)
        {
            var entry = Context.Entry(entity);

            // need to stop it from being tracked after it's been added
            entry.State = EntityState.Detached;
        }
    }
}
