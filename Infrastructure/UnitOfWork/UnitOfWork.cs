using Application.Abstractions.IRepositories;
using Application.Abstractions.IUnitOfWork;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly Lazy<IItemRepository> _itemRepository;
    private readonly Lazy<ICategoryRepository> _categoryRepository;
    private readonly Lazy<IUserRepository> _userRepository;
    private readonly Lazy<ITradeRequestRepository> _tradeRequestRepository;
    private readonly Lazy<IReviewRepository> _reviewRepository;
    private readonly Lazy<IChatRepository> _chatRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        _itemRepository = new Lazy<IItemRepository>(new ItemRepository(_context));
        _chatRepository =new Lazy<IChatRepository>(new ChatRepository(_context));
        _categoryRepository = new Lazy<ICategoryRepository>(new CategoryRepository(_context));
        _userRepository = new Lazy<IUserRepository>(new UserRepository(_context)); 
        _tradeRequestRepository = new Lazy<ITradeRequestRepository>(new TradeRequestRepository(_context));
        _reviewRepository = new Lazy<IReviewRepository>(new ReviewRepository(_context));
    }
    public IItemRepository ItemRepository => _itemRepository.Value;
    public IChatRepository ChatRepository => _chatRepository.Value;
    public ICategoryRepository CategoryRepository => _categoryRepository.Value;
    public IUserRepository UserRepository => _userRepository.Value; 
    public ITradeRequestRepository TradeRequestRepository => _tradeRequestRepository.Value;
    public IReviewRepository ReviewRepository => _reviewRepository.Value;
    public async Task SaveAsync()
     => await _context.SaveChangesAsync();
}
