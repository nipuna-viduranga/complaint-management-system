using System.ComponentModel.DataAnnotations;
using ComplaintSystem.Domain.Enums;

namespace ComplaintSystem.Application.DTOs;

public class ComplaintDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ComplaintStatus Status { get; set; }
    public Priority Priority { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string? AttachmentUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateComplaintDto
{
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    public Guid CategoryId { get; set; }
}

public class UpdateComplaintStatusDto
{
    public Guid Id { get; set; }
    public ComplaintStatus Status { get; set; }
    public Priority? Priority { get; set; }
}
