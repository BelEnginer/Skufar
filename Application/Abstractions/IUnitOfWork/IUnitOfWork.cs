using Application.Abstractions.IRepositories;

namespace Application.Abstractions.IUnitOfWork;

public interface IUnitOfWork
{
    IItemRepository ItemRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    IUserRepository UserRepository { get; }
    IReviewRepository ReviewRepository { get; }
    ITradeRequestRepository TradeRequestRepository { get; } 
    Task SaveAsync();
}