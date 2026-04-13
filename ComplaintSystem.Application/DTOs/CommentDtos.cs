using System.ComponentModel.DataAnnotations;

namespace ComplaintSystem.Application.DTOs;

public class CommentDto
{
    public Guid Id { get; set; }
    public Guid ComplaintId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsInternalOnly { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCommentDto
{
    [Required]
    public Guid ComplaintId { get; set; }
    [Required]
    public string Content { get; set; } = string.Empty;
    public bool IsInternalOnly { get; set; } = false;
}
