using System.Linq.Expressions;
using Application.Abstractions.IRepositories;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public abstract class BaseRepository<T>(ApplicationDbContext context) : IBaseRepository<T>
    where T : class
{
    protected readonly ApplicationDbContext Context = context;

    public IQueryable<T> GetByFilter(Expression<Func<T, bool>> filter) => 
        Context.Set<T>()
            .Where(filter);
    public IQueryable<T> GetAll() => 
        Context.Set<T>();
    
    public void Delete(T entity) => 
        Context.Set<T>()
        .Remove(entity);
    
    public void Update(T entity) =>  
        Context.Set<T>()
        .Update(entity);
}