using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ComplaintSystem.Domain.Entities;
using ComplaintSystem.Domain.Interfaces;
using ComplaintSystem.Infrastructure.Data;

namespace ComplaintSystem.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    
    public Repository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<T?> GetByIdAsync(Guid id) => await _context.Set<T>().FindAsync(id);
    
    public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();
    
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) => await _context.Set<T>().Where(predicate).ToListAsync();
    
    public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);
    
    public void Update(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Set<T>().Update(entity);
    }
    
    public void Remove(T entity) => _context.Set<T>().Remove(entity);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    
    public IRepository<Complaint> Complaints { get; private set; }
    public IRepository<Category> Categories { get; private set; }
    public IRepository<Department> Departments { get; private set; }
    public IRepository<ComplaintLog> ComplaintLogs { get; private set; }
    public IRepository<Feedback> Feedbacks { get; private set; }
    public IRepository<Comment> Comments { get; private set; }
    
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Complaints = new Repository<Complaint>(_context);
        Categories = new Repository<Category>(_context);
        Departments = new Repository<Department>(_context);
        ComplaintLogs = new Repository<ComplaintLog>(_context);
        Feedbacks = new Repository<Feedback>(_context);
        Comments = new Repository<Comment>(_context);
    }
    
    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();
    
    public void Dispose() => _context.Dispose();
}
