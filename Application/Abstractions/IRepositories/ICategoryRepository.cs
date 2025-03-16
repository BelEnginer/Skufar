using Domain.Entites;

namespace Application.Abstractions.IRepositories;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllCategoriesAsync(CancellationToken ct);
    Task<Category?> GetCategoryByIdAsync(Guid categoryId, CancellationToken ct);
}