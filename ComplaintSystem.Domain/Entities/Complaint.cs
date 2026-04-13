using ComplaintSystem.Domain.Enums;

namespace ComplaintSystem.Domain.Entities;

public class Complaint : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ComplaintStatus Status { get; set; } = ComplaintStatus.Open;
    public Priority Priority { get; set; } = Priority.Low;
    
    // User reporting the complaint
    public string UserId { get; set; } = string.Empty;
    
    // Foreign keys
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    
    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }
    
    // Attachments metadata (e.g. JSON strings or separate File entity)
    public string? AttachmentUrl { get; set; }
    
    public ICollection<ComplaintLog> Logs { get; set; } = new List<ComplaintLog>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public Feedback? Feedback { get; set; }
}
