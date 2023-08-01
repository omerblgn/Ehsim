using webapi.Entity;


namespace webapi.Data.Interface
{
    public interface IRepository<TEntity> : IQueryable<TEntity> where TEntity : BaseEntity
    {
        void Insert(TEntity entity);
        void InsertRange(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void InsertOrUpdate(TEntity entity);
        void DetachAllEntires();
        void Delete(params TEntity[] entities);
        void Delete(int Id);
        void SoftDelete(int Id);
        TEntity GetById(int Id);
        IQueryable<TEntity> GetAll();
    }
}


