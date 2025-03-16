using Application.Common;
using Application.DTOs.GetDtos;

namespace Application.Abstractions.IServices;

public interface ICategoryService
{
    Task<Result<List<CategoryDto>>> GetAllCategoriesAsync(CancellationToken ct); 
    Task<Result<CategoryDto>> GetCategoryByIdAsync(Guid categoryId,CancellationToken ct);
}