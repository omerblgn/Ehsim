using webapi.Entity;


namespace webapi.Data.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();
        MainDbContext GetContext();
        IRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
    }
}


