using Mapster;
using ComplaintSystem.Application.DTOs;
using ComplaintSystem.Application.Interfaces;
using ComplaintSystem.Domain.Entities;
using ComplaintSystem.Domain.Interfaces;

namespace ComplaintSystem.Application.Services;

public class CommentService : ICommentService
{
    private readonly IUnitOfWork _unitOfWork;

    public CommentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CommentDto> AddCommentAsync(CreateCommentDto dto, string userId, string userName)
    {
        var comment = new Comment
        {
            ComplaintId = dto.ComplaintId,
            Content = dto.Content,
            UserId = userId,
            UserName = userName,
            IsInternalOnly = dto.IsInternalOnly
        };

        await _unitOfWork.Comments.AddAsync(comment);
        
        // Log the action
        var log = new ComplaintLog
        {
            ComplaintId = dto.ComplaintId,
            ActionDetails = $"Comment added by {userName} ({(dto.IsInternalOnly ? "Internal" : "Public")}).",
            ModifiedByUserId = userId
        };
        await _unitOfWork.ComplaintLogs.AddAsync(log);

        await _unitOfWork.CompleteAsync();

        return comment.Adapt<CommentDto>();
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsByComplaintIdAsync(Guid complaintId, bool includeInternal = false)
    {
        var comments = await _unitOfWork.Comments.FindAsync(c => c.ComplaintId == complaintId);
        
        if (!includeInternal)
        {
            comments = comments.Where(c => !c.IsInternalOnly);
        }

        return comments.OrderBy(c => c.CreatedAt).Adapt<IEnumerable<CommentDto>>();
    }
}
