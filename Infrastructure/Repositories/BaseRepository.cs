using System.Linq.Expressions;
using Application.Abstractions.IRepositories;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal abstract class BaseRepository<T>(ApplicationDbContext context) : IBaseRepository<T>
    where T : class
{
    protected readonly ApplicationDbContext Context = context;
    protected IQueryable<T> Query => Context.Set<T>();
    public async Task<List<T>> GetAllAsync(CancellationToken ct) =>
        await Query
            .ToListAsync(ct);
    
    public void Delete(T entity) => 
        Context.Set<T>()
        .Remove(entity);
    
    public void Update(T entity) =>  
        Context.Set<T>()
        .Update(entity);
}