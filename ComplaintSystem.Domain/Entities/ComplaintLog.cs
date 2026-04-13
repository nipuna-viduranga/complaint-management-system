using ComplaintSystem.Domain.Enums;

namespace ComplaintSystem.Domain.Entities;

public class ComplaintLog : BaseEntity
{
    public Guid ComplaintId { get; set; }
    public Complaint Complaint { get; set; } = null!;
    
    // Who made the update (Admin, Staff, or System)
    public string ModifiedByUserId { get; set; } = string.Empty;
    public string ActionDetails { get; set; } = string.Empty;
    
    public ComplaintStatus? OldStatus { get; set; }
    public ComplaintStatus? NewStatus { get; set; }
}
