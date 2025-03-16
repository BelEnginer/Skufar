using Application.Abstractions.IUnitOfWork;
using Application.Common;
using Application.DTOs.GetDtos;
using Application.DTOs.PostDtos;
using Application.DTOs.UpdateDtos;
using Application.Exceptions;
using Application.IServices;
using AutoMapper;
using Domain.Entites;
using Domain.Enums;
using Infrastructure.Helpers;
using Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace Infrastructure.Services;

public class ItemService(
    IUnitOfWork _repository,
    IMapper _mapper,
    IOptions<MinioSetting> _minioSetting,
    IMinioClient _minioClient,
    ILogger<ItemService> _logger)
    : IItemService
{
    private readonly MinioSetting _minioSetting = _minioSetting.Value;

    public async Task<Result<List<ItemDto>>> GetAllItemsAsync(bool includeImages = false, CancellationToken ct = default)
    {
        var allItems = await _repository.ItemRepository.GetAllItemsAsync(ct);
        _logger.LogInformation("Retrieved {ItemCount} items", allItems.Count);
        return Result<List<ItemDto>>.Success(MapItems(allItems, includeImages));
    }

    public async Task<Result<List<ItemDto>>> GetItemsByCategoryAsync(Guid categoryId, bool includeImages = false, CancellationToken ct = default)
    {
        var items = await _repository.ItemRepository.GetItemsByCategoryAsync(categoryId, ct);
        _logger.LogInformation("there were items from the category with id {categoryId}: {items.Count}",items.Count,categoryId);
        return Result<List<ItemDto>>.Success(MapItems(items, includeImages));
    }

    public async Task<Result<ItemDto>> GetItemByIdAsync(Guid id, CancellationToken ct = default)
    {
        var item = await _repository.ItemRepository.GetItemByIdAsync(id, ct);
        if (item == null)
        {
            _logger.LogWarning("Item with id {id} not found", id);
            return Result<ItemDto>.Failure("Item not found", ErrorType.NotFound);
        }
        _logger.LogInformation("Item with id {id} found", id);
        return Result<ItemDto>.Success(_mapper.Map<ItemDto>(item));
    }

   public async Task<Result<ItemDto>> CreateItemAsync(ItemPostDto itemPostDto, CancellationToken ct = default)
    {
        return await ErrorHandlingHelper.ExecuteAsync(async () =>
        {
            var category = await _repository.CategoryRepository.GetCategoryByIdAsync(itemPostDto.CategoryId, ct);
            var owner = await _repository.UserRepository.GetUserByIdAsync(itemPostDto.OwnerId, ct);

            if (owner == null)
            {
                _logger.LogError("Unauthorized access attempt by user {OwnerId}", itemPostDto.OwnerId);
                return Result<ItemDto>.Failure("Owner not authorized", ErrorType.Unauthorized);
            }

            if (category == null)
            {
                _logger.LogWarning("Category with id {CategoryId} not found", itemPostDto.CategoryId);
                return Result<ItemDto>.Failure("Category not found", ErrorType.NotFound);
            }

            var newItem = _mapper.Map<Item>(itemPostDto);
            newItem.Category = category;
            newItem.Owner = owner;
            newItem.Images = new List<ItemImage>();

            try
            {
                var isExist = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_minioSetting.BucketName), ct);
                if (!isExist)
                {
                    _logger.LogInformation("Bucket {BucketName} does not exist. Creating a new one...", _minioSetting.BucketName);
                    await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_minioSetting.BucketName), ct);
                }

                string? previewImagePath = null;
                foreach (var file in itemPostDto.Images)
                {
                    await using var stream = file.OpenReadStream();
                    var objectName = $"{itemPostDto.Name}:{Guid.NewGuid()}_{file.FileName}";

                    await _minioClient.PutObjectAsync(
                        new PutObjectArgs()
                            .WithBucket(_minioSetting.BucketName)
                            .WithObject(objectName)
                            .WithStreamData(stream)
                            .WithObjectSize(file.Length)
                            .WithContentType(file.ContentType),
                        ct);

                    var imagePath = $"{_minioSetting.Endpoint}/{_minioSetting.BucketName}/{objectName}";
                    newItem.Images.Add(new ItemImage { Id = Guid.NewGuid(), Item = newItem, ImagePath = imagePath });
                    previewImagePath ??= imagePath;
                }

                newItem.PreviewImagePath = previewImagePath;
            }
            catch (MinioException ex)
            {
                _logger.LogError(ex, "Failed to upload photo for item {ItemId} to Minio", newItem.Id);
                throw new InfrastructureException("Failed to upload photo in Minio", ex);
            }

            await _repository.ItemRepository.CreateItemAsync(newItem, ct);
            await _repository.SaveAsync();
            _logger.LogInformation("Item with id {ItemId} added to category {CategoryId}", newItem.Id, newItem.Category.Id);
            return Result<ItemDto>.Success(_mapper.Map<ItemDto>(newItem));
        }, _logger);
    }


    public async Task<Result<Unit>> UpdateItemAsync(Guid idItemToUpdate, ItemUpdateDto itemUpdateDto, CancellationToken ct)
    {
        return await ErrorHandlingHelper.ExecuteAsync(async () =>
        {
            var itemEntity = await _repository.ItemRepository.GetItemByIdAsync(idItemToUpdate, ct);
            if (itemEntity == null)
            {
                _logger.LogWarning("Item with id {id} not found", idItemToUpdate);
                return Result<Unit>.Failure("Item not found", ErrorType.NotFound);
            }

            /*добавить обновление фото*/
            _mapper.Map(itemUpdateDto, itemEntity);
            await _repository.ItemRepository.UpdateItemAsync(itemEntity, ct);
            await _repository.SaveAsync();
            _logger.LogInformation("Item with id {id} updated", idItemToUpdate);
            return Result<Unit>.Success(Unit.Value);
        }, _logger);
    }

    public async Task<Result<Unit>> DeleteItemAsync(Guid idItemToDelete, CancellationToken ct)
    {
        return await ErrorHandlingHelper.ExecuteAsync(async () =>
        {
            var itemToDelete = await _repository.ItemRepository.GetItemByIdAsync(idItemToDelete, ct);
            if (itemToDelete == null)
            {
                _logger.LogWarning("Item with id {ItemId} not found", idItemToDelete);
                return Result<Unit>.Failure("Item not found", ErrorType.NotFound);
            }

            var images = itemToDelete.Images
                .Select(x => x.ImagePath.Replace($"{_minioSetting.Endpoint}/{_minioSetting.BucketName}/", ""))
                .ToList();
            await _minioClient.RemoveObjectsAsync(new RemoveObjectsArgs()
                .WithBucket(_minioSetting.BucketName)
                .WithObjects(images), ct);

            _logger.LogInformation("Deleted {ImageCount} images from Minio for item {ItemId}", images.Count, idItemToDelete);

            _repository.ItemRepository.DeleteItem(itemToDelete);
            await _repository.SaveAsync();
            _logger.LogInformation("Item with id {ItemId} deleted", idItemToDelete);
            return Result<Unit>.Success(Unit.Value);
        }, _logger);
    }
    
    private List<ItemDto> MapItems(IEnumerable<Item> items, bool includeImages) =>
        items.Select(item => MapItem(item, includeImages)).ToList();
    private ItemDto MapItem(Item item, bool includeImages)
    {
        var itemDto = _mapper.Map<ItemDto>(item);
        itemDto.ImagePaths = includeImages 
            ? item.Images.Select(img => img.ImagePath).ToList() 
            : [];
        return itemDto;
    }
}

