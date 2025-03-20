using Application.Abstractions.IServices;
using Application.Abstractions.IUnitOfWork;
using Application.Common;
using Application.DTOs.GetDtos;
using AutoMapper;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class CategoryService(IUnitOfWork _repository,
    IMapper _mapper,
    ILogger<CategoryService> _logger) : ICategoryService
{
    public async Task<Result<List<CategoryDto>>> GetAllCategoriesAsync(CancellationToken ct)
    {
        var allCategories = await _repository.CategoryRepository.GetAllCategoriesAsync(ct);

        _logger.LogInformation("Found {Count} categories from DB", allCategories?.Count ?? 0);

        if (allCategories is null || allCategories.Count == 0)
        {
            _logger.LogWarning("No categories found, returning empty list.");
            return Result<List<CategoryDto>>.Success(new List<CategoryDto>());
        }

        var mappedCategories = _mapper.Map<List<CategoryDto>>(allCategories);
        _logger.LogInformation("Mapped {Count} categories to DTOs", mappedCategories?.Count ?? 0);

        return Result<List<CategoryDto>>.Success(mappedCategories ?? new List<CategoryDto>());
    }

    public async Task<Result<CategoryDto>> GetCategoryByIdAsync(Guid id,CancellationToken ct)
    {
        var category = await _repository.CategoryRepository.GetCategoryByIdAsync(id,ct);
        if (category == null)
        {
            _logger.LogWarning("Category with id {id} not found",id);
            return Result<CategoryDto>.Failure("Category not found", ErrorType.NotFound);
        } 
        _logger.LogInformation("Category with id {id} found",id);
        return Result<CategoryDto>.Success(_mapper.Map<CategoryDto>(category));
    }
}