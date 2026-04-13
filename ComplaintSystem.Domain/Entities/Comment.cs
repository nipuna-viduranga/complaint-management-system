namespace ComplaintSystem.Domain.Entities;

public class Comment : BaseEntity
{
    public Guid ComplaintId { get; set; }
    public Complaint Complaint { get; set; } = null!;
    
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    
    // Whether this comment is visible to the customer (User) or internal only
    public bool IsInternalOnly { get; set; } = false;
}
