using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using webapi.Data.Interface;
using webapi.Entity;

namespace webapi.Data.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool _disposed;
        protected MainDbContext _dataContext;
        protected Dictionary<string, dynamic> _repositories;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected int _UserId;


        public UnitOfWork(MainDbContext dataContext, IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _repositories = new Dictionary<string, dynamic>();
            _httpContextAccessor = httpContextAccessor;
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var userIdString = _httpContextAccessor.HttpContext.User?.Claims?.First(x => x.Type == ClaimTypes.Actor).Value;
                if (!string.IsNullOrEmpty(userIdString))
                    int.TryParse(userIdString, out _UserId);
            }
            if (!(_UserId > 0))
                _UserId = 0;

        }
        public UnitOfWork(MainDbContext dataContext, int UserId)
        {
            _dataContext = dataContext;
            _repositories = new Dictionary<string, dynamic>();
            _UserId = UserId;
        }

        public MainDbContext GetContext()
        {
            return _dataContext;
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            if (_repositories == null)
            {
                _repositories = new Dictionary<string, dynamic>();
            }

            var type = typeof(TEntity).Name;

            if (_repositories.ContainsKey(type))
            {
                return (IRepository<TEntity>)_repositories[type];
            }

            var repositoryType = typeof(Repository<>);
            _repositories.Add(type, Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _dataContext));

            return _repositories[type];
        }

        public int SaveChanges()
        {
            var models = _dataContext.ChangeTracker.Entries()
               .Where(x => x.Entity is BaseEntity
               && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var model in models)
            {
                BaseEntity table = model.Entity as BaseEntity;
                if (table != null)
                {
                    if (model.State == EntityState.Added)
                    {
                        table.IsDeleted = false;
                        table.Creator = _UserId;
                        table.CreationDate = DateTime.Now;
                        table.UpdateDate = null;
                    }
                    else if (model.State == EntityState.Modified)
                    {
                        try
                        {
                            var date = model.GetDatabaseValues()?.GetValue<DateTime>("CreationDate");
                            if (date != null)
                            {
                                _dataContext.Entry(table).Property(x => x.CreationDate).CurrentValue = date.Value;
                                _dataContext.Entry(table).Property(x => x.CreationDate).OriginalValue = date.Value;
                            }
                            _dataContext.Entry(table).Property(x => x.CreationDate).IsModified = false;
                        }
                        catch (Exception)
                        {
                            table.IsDeleted = false;
                            table.Creator = _UserId;
                            table.CreationDate = DateTime.Now;
                        }

                        table.UpdateDate = DateTime.Now;
                        table.Updater = _UserId;

                    }
                }
            }
            return _dataContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dataContext.SaveChangesAsync();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try
                    {

                        if (_dataContext != null)
                        {
                            _dataContext.Dispose();
                            _dataContext = null;
                        }
                    }
                    ///TODO log ve tail veya solarwinds'e bildirme eklenecek
                    catch (ObjectDisposedException)
                    {
                        // do nothing, the objectContext has already been disposed
                    }
                }
                _disposed = true;
            }
        }
    }
}


