using System.Linq.Expressions;
using ComplaintSystem.Domain.Entities;

namespace ComplaintSystem.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}

public interface IUnitOfWork : IDisposable
{
    IRepository<Complaint> Complaints { get; }
    IRepository<Category> Categories { get; }
    IRepository<Department> Departments { get; }
    IRepository<ComplaintLog> ComplaintLogs { get; }
    IRepository<Feedback> Feedbacks { get; }
    IRepository<Comment> Comments { get; }
    
    Task<int> CompleteAsync();
}
