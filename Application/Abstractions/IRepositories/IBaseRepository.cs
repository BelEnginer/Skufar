using System.Linq.Expressions;

namespace Application.Abstractions.IRepositories;

public interface IBaseRepository<T> where T : class
{
   Task<List<T>> GetAllAsync(CancellationToken ct);
   void Delete(T entity);
   void Update(T entity);
}