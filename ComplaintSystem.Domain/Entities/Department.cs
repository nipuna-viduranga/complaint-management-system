namespace ComplaintSystem.Domain.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
}
