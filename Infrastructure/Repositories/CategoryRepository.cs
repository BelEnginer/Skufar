using Application.Abstractions.IRepositories;
using Domain.Models;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal sealed class CategoryRepository(ApplicationDbContext context) : BaseRepository<Category>(context), ICategoryRepository

{
    public async Task<List<Category>> GetAllCategoriesAsync(CancellationToken ct) =>
        await GetAllAsync(ct);

    public async Task<Category?> GetCategoryByIdAsync(Guid id, CancellationToken ct) =>
        await Query
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
}