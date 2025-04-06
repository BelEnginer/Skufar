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
        _logger.LogInformation("Found {allCategories.Count} Categories",allCategories.Count);
        return Result<List<CategoryDto>>.Success(_mapper.Map<List<CategoryDto>>(allCategories)); 
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