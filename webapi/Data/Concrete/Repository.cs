using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Linq.Expressions;
using webapi.Data.Interface;
using webapi.Entity;

namespace   webapi.Data.Concrete
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private DbSet<TEntity> _dbSet;
        private MainDbContext _dbContext;

        public Repository(MainDbContext context)
        {
            _dbContext = context;
            if (_dbContext != null)
            {
                _dbSet = _dbContext.Set<TEntity>();
            }

        }
        public void Delete(params TEntity[] entities)
        {
            foreach (TEntity a in entities)
                Delete(a.Id);
        }

        public void Delete(int Id)
        {
            _dbSet.Remove(GetById(Id));
        }
        public void SoftDelete(int Id)
        {
            var entity = GetById(Id);
            entity.IsDeleted = true;
            _dbSet.Update(entity);
        }
        public IQueryable<TEntity> GetAll()
        {
            return _dbSet;
        }
        public IQueryable<TEntity> GetAllActive()
        {
            return _dbSet.Where(i => i.IsDeleted == false);
        }

        public TEntity GetById(int Id)
        {
            return _dbSet.Find(Id);
        }

        public void Insert(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _dbSet.Add(entity);
        }

        public void InsertRange(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }
        public void InsertOrUpdate(TEntity entity)
        {
            if (entity.Id > 0)
                _dbSet.Update(entity);
            else
                _dbSet.Add(entity);
        }
        public void DetachAllEntires()
        {
            var changedEntries = _dbContext.ChangeTracker.Entries<TEntity>()
             .Where(e =>
                    e.State == EntityState.Unchanged ||
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified ||
                    e.State == EntityState.Deleted
                    )
                    .ToList();
            foreach (var change in changedEntries)
                change.State = EntityState.Detached;

        }
        public IEnumerator<TEntity> GetEnumerator()
        {
            return _dbSet.AsNoTracking().AsEnumerable<TEntity>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dbSet.AsNoTracking().AsEnumerable().GetEnumerator();
        }

        public Type ElementType
        {
            get { return typeof(TEntity); }
        }

        public Expression Expression
        {
            get { return _dbSet.AsNoTracking().AsQueryable<TEntity>().Expression; }
        }

        public IQueryProvider Provider
        {
            get { return _dbSet.AsNoTracking().AsQueryable<TEntity>().Provider; }
        }



    }
}

