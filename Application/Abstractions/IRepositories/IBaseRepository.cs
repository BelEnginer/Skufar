using System.Linq.Expressions;

namespace Application.Abstractions.IRepositories;

public interface IBaseRepository<T> where T : class
{
   IQueryable<T?> GetByFilter(Expression<Func<T, bool>> filter);
   IQueryable<T> GetAll(); 
   Task CreateAsync(T entity,CancellationToken ct);
   void Delete(T entity);
   void Update(T entity);
}