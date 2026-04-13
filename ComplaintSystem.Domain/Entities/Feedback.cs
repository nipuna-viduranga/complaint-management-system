namespace ComplaintSystem.Domain.Entities;

public class Feedback : BaseEntity
{
    public Guid ComplaintId { get; set; }
    public Complaint Complaint { get; set; } = null!;
    
    // Rating 1 to 5
    public int Rating { get; set; }
    public string Comments { get; set; } = string.Empty;
}
