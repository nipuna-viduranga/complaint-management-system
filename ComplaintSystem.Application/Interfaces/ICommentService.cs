using ComplaintSystem.Application.DTOs;

namespace ComplaintSystem.Application.Interfaces;

public interface ICommentService
{
    Task<CommentDto> AddCommentAsync(CreateCommentDto dto, string userId, string userName);
    Task<IEnumerable<CommentDto>> GetCommentsByComplaintIdAsync(Guid complaintId, bool includeInternal = false);
}
