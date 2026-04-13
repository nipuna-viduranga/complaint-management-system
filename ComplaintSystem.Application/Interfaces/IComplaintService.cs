using ComplaintSystem.Application.DTOs;
using ComplaintSystem.Domain.Enums;

namespace ComplaintSystem.Application.Interfaces;

public interface IComplaintService
{
    Task<ComplaintDto> CreateComplaintAsync(CreateComplaintDto dto, string userId);
    Task<ComplaintDto?> GetComplaintByIdAsync(Guid id);
    Task<IEnumerable<ComplaintDto>> GetAllComplaintsAsync(ComplaintStatus? status = null);
    Task<IEnumerable<ComplaintDto>> GetComplaintsByUserIdAsync(string userId);
    Task<bool> UpdateComplaintStatusAsync(UpdateComplaintStatusDto dto, string modifiedByUserId);
    Task<bool> AssignDepartmentAsync(Guid complaintId, Guid departmentId, string modifiedByUserId);
}
