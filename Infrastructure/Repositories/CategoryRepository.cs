using Application.Abstractions.IRepositories;
using Domain.Entites;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CategoryRepository(ApplicationDbContext context) : BaseRepository<Category>(context), ICategoryRepository

{
    public async Task<List<Category>> GetAllCategoriesAsync(CancellationToken ct) => 
        await GetAll()
            .AsNoTracking()
            .ToListAsync(ct);
    
    public async Task<Category?> GetCategoryByIdAsync(Guid id,CancellationToken ct) => 
        await GetByFilter(i => i.Id == id)
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);
}