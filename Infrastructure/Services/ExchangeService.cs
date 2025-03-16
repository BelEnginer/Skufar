using Application.Abstractions.IUnitOfWork;
using Application.Common;
using Application.DTOs.GetDtos;
using Application.DTOs.PostDtos;
using Application.IServices;
using AutoMapper;
using Domain.Entites;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Infrastructure.Services;

public class ExchangeService(IUnitOfWork _repository,
    IMapper _mapper, 
    ILogger<ExchangeService> _logger) : IExchangeService
{
    public async Task<Result<Unit>> RequestExchangeAsync(TradeRequestPostDto tradeRequestPostDto, CancellationToken ct)
    {
        var areItemsValid = await AreItemsValidAsync(tradeRequestPostDto.ItemRequestedId, tradeRequestPostDto.ItemOfferedId, ct);
        if (!areItemsValid)
        {
            _logger.LogWarning("Items to exchange are not valid");
            return Result<Unit>.Failure("One or both items not found.", ErrorType.NotFound);
        }

        var isOwner = await _repository.ItemRepository.IsItemOwnedByUser(tradeRequestPostDto.ItemOfferedId, tradeRequestPostDto.SenderId,ct);
        if (!isOwner)
        {
            _logger.LogWarning("Items are not owned by user");
            return Result<Unit>.Failure("User does not own this exchange.", ErrorType.Conflict);
        }

        var existingRequest = await _repository.TradeRequestRepository.ExistsAsync(tradeRequestPostDto.ItemOfferedId
            ,tradeRequestPostDto.ItemRequestedId
            ,tradeRequestPostDto.SenderId,ct);
        if (existingRequest)
        {
            _logger.LogWarning("Request already exists");
            return Result<Unit>.Failure("Exchange request already exists.", ErrorType.Conflict);
        }

        var newRequest = _mapper.Map<TradeRequest>(tradeRequestPostDto);
        newRequest.Status = Status.Pending;
        await _repository.TradeRequestRepository.CreateTradeRequestAsync(newRequest,ct);
        await _repository.SaveAsync();
        _logger.LogInformation("New exchange request created with id {TradeRequestId}", newRequest.Id);
        return Result<Unit>.Success(Unit.Value);
    }

    public async Task<Result<Unit>> AcceptExchangeAsync(TradeRequestDto tradeRequestDto, CancellationToken ct)
    {
        var validationResult = await ValidateTradeRequestAsync(tradeRequestDto.Id, ct);
        if (!validationResult.IsSuccess)
        {
            _logger.LogWarning("Invalid trade request with id {tradeRequestDto.Id}", tradeRequestDto.Id);
            return Result<Unit>.Failure(validationResult.ErrorMessage,
                validationResult.ErrorType ?? ErrorType.Conflict);
        }

        var incomingTradeRequest = validationResult.Value;
        if (incomingTradeRequest.ReceiverId != tradeRequestDto.ReceiverId)
        {
            _logger.LogWarning("Incorrect recipient of the exchange request witch id {tradeRequestDto.Id}", tradeRequestDto.Id);
            return Result<Unit>.Failure("Only the receiver can accept this exchange request.", ErrorType.Conflict);
        }

        var areItemsValid = await AreItemsValidAsync(tradeRequestDto.ItemRequestedId, tradeRequestDto.ItemOfferedId, ct);
        if (!areItemsValid)
        {
            _logger.LogWarning("Items to exchange are not valid");
            return Result<Unit>.Failure("One or both items not found.", ErrorType.NotFound);
        }
        incomingTradeRequest.Status = Status.Accepted;
        await _repository.SaveAsync();
        await _repository.ItemRepository.ChangeOwnerOfItemAsync(tradeRequestDto.ItemOfferedId, tradeRequestDto.ItemRequestedId, ct);
        await _repository.SaveAsync();
        _logger.LogInformation("Exchange request with id {tradeRequestDto.Id} accepted", tradeRequestDto.Id);
        return Result<Unit>.Success(Unit.Value);
    }



    public async Task<Result<Unit>> RejectExchangeAsync(TradeRequestDto tradeRequestDto, CancellationToken ct)
    {
        var validationResult = await ValidateTradeRequestAsync(tradeRequestDto.Id, ct);
        if (!validationResult.IsSuccess)
        {
            _logger.LogWarning("Invalid trade request with id {tradeRequestDto.Id}", tradeRequestDto.Id);
            return Result<Unit>.Failure(validationResult.ErrorMessage,
                validationResult.ErrorType ?? ErrorType.Conflict);
        }

        var incomingTradeRequest = validationResult.Value;
        if (incomingTradeRequest.ReceiverId != tradeRequestDto.ReceiverId)
        {
            _logger.LogWarning("Incorrect recipient of the exchange request witch id {tradeRequestDto.Id}", tradeRequestDto.Id);
            return Result<Unit>.Failure("Only the receiver can reject the exchange.", ErrorType.Conflict);
        }

        incomingTradeRequest.Status = Status.Rejected;
        await _repository.SaveAsync();
        _logger.LogInformation("Exchange request with id {tradeRequestDto.Id} rejected", tradeRequestDto.Id);
        return Result<Unit>.Success(Unit.Value);
    }


    public async Task<Result<Unit>> CancelExchangeAsync(TradeRequestDto tradeRequestDto, CancellationToken ct)
    {
        var validationResult = await ValidateTradeRequestAsync(tradeRequestDto.Id, ct);
        if (!validationResult.IsSuccess)
        {
            _logger.LogWarning("Invalid trade request with id {tradeRequestDto.Id}", tradeRequestDto.Id);
            return Result<Unit>.Failure(validationResult.ErrorMessage,
                validationResult.ErrorType ?? ErrorType.Conflict);
        }

        var incomingTradeRequest = validationResult.Value;
        if (incomingTradeRequest.SenderId != tradeRequestDto.SenderId)
        {
            _logger.LogWarning("Incorrect sender of the exchange request witch id {tradeRequestDto.Id}", tradeRequestDto.Id);
            return Result<Unit>.Failure("Only the sender can cancel the exchange.", ErrorType.Conflict);
        }

        incomingTradeRequest.Status = Status.Cancelled;
        await _repository.SaveAsync();
        _logger.LogInformation("Exchange request with id {tradeRequestDto.Id} canceled", tradeRequestDto.Id);
        return Result<Unit>.Success(Unit.Value);
    }


    public async Task<Result<List<TradeRequestDto>>> GetUserTradeRequestsAsync(Guid userId, CancellationToken ct)
    {
        var tradeRequests = await _repository.TradeRequestRepository.GetTradeRequestsByUserIdAsync(userId,ct);
        _logger.LogInformation("Found {TradeRequestCount} trade requests for user {UserId}", tradeRequests.Count, userId);
        return Result<List<TradeRequestDto>>.Success(_mapper.Map<List<TradeRequestDto>>(tradeRequests));
    }
    
    private async Task<bool> AreItemsValidAsync(Guid itemRequestedId, Guid itemOfferedId, CancellationToken ct)
    {
        var items = await _repository.ItemRepository.GetItemsByIdsAsync([itemRequestedId, itemOfferedId], ct);
        var itemRequested = items.FirstOrDefault(item => item?.Id == itemRequestedId);
        var itemOffered = items.FirstOrDefault(item => item?.Id == itemOfferedId);
        return itemRequested != null && itemOffered != null;
    }
    private async Task<Result<TradeRequest>> ValidateTradeRequestAsync(Guid tradeRequestId, CancellationToken ct)
    {
        var tradeRequest = await _repository.TradeRequestRepository.GetTradeRequestByIdAsync(tradeRequestId, ct);
        if (tradeRequest == null)
        {
            _logger.LogWarning("Trade request with id {tradeRequestId} not found", tradeRequestId);
            return Result<TradeRequest>.Failure("Exchange request not found.", ErrorType.NotFound);
        }

        if (tradeRequest.Status != Status.Pending)
        {
            _logger.LogWarning("Trade request with id {TradeRequestId} has a status {Status}, expected: Pending",
                tradeRequestId, tradeRequest.Status);
            return Result<TradeRequest>.Failure("Exchange request is not pending.", ErrorType.Conflict);
        }
        _logger.LogInformation("Trade request {TradeRequestId} is valid for processing", tradeRequestId);
        return Result<TradeRequest>.Success(tradeRequest);
    }
}