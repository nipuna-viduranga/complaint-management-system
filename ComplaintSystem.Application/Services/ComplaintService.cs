using Mapster;
using ComplaintSystem.Application.DTOs;
using ComplaintSystem.Application.Interfaces;
using ComplaintSystem.Domain.Entities;
using ComplaintSystem.Domain.Enums;
using ComplaintSystem.Domain.Interfaces;

namespace ComplaintSystem.Application.Services;

public class ComplaintService : IComplaintService
{
    private readonly IUnitOfWork _unitOfWork;

    public ComplaintService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ComplaintDto> CreateComplaintAsync(CreateComplaintDto dto, string userId)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
        
        var complaint = new Complaint
        {
            Title = dto.Title,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            UserId = userId,
            Status = ComplaintStatus.Open,
            Priority = CalculateSmartPriority(category?.Name ?? "")
        };

        await _unitOfWork.Complaints.AddAsync(complaint);
        
        var log = new ComplaintLog
        {
            ComplaintId = complaint.Id,
            ActionDetails = "Complaint filed.",
            NewStatus = ComplaintStatus.Open,
            ModifiedByUserId = userId
        };
        await _unitOfWork.ComplaintLogs.AddAsync(log);
        
        await _unitOfWork.CompleteAsync();

        return complaint.Adapt<ComplaintDto>();
    }

    public async Task<ComplaintDto?> GetComplaintByIdAsync(Guid id)
    {
        var complaints = await _unitOfWork.Complaints.FindAsync(c => c.Id == id);
        var complaint = complaints?.FirstOrDefault();
        if (complaint == null) return null;

        var dto = complaint.Adapt<ComplaintDto>();
        var category = await _unitOfWork.Categories.GetByIdAsync(complaint.CategoryId);
        dto.CategoryName = category?.Name ?? "Unknown";

        if (complaint.DepartmentId.HasValue)
        {
            var dept = await _unitOfWork.Departments.GetByIdAsync(complaint.DepartmentId.Value);
            dto.DepartmentName = dept?.Name;
        }

        return dto;
    }

    public async Task<IEnumerable<ComplaintDto>> GetAllComplaintsAsync(ComplaintStatus? status = null)
    {
        var complaints = status.HasValue 
            ? await _unitOfWork.Complaints.FindAsync(c => c.Status == status.Value)
            : await _unitOfWork.Complaints.GetAllAsync();

        var dtos = complaints.Adapt<IEnumerable<ComplaintDto>>();
        
        // Manual mapping for names since Mapster won't fetch them from separate repos automatically without includes
        foreach (var dto in dtos)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
            dto.CategoryName = category?.Name ?? "Unknown";
            
            if (dto.DepartmentId.HasValue)
            {
                var dept = await _unitOfWork.Departments.GetByIdAsync(dto.DepartmentId.Value);
                dto.DepartmentName = dept?.Name;
            }
        }

        return dtos;
    }

    public async Task<IEnumerable<ComplaintDto>> GetComplaintsByUserIdAsync(string userId)
    {
        var complaints = await _unitOfWork.Complaints.FindAsync(c => c.UserId == userId);
        var dtos = complaints.Adapt<IEnumerable<ComplaintDto>>();
        
        foreach (var dto in dtos)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
            dto.CategoryName = category?.Name ?? "Unknown";
        }
        
        return dtos;
    }

    public async Task<bool> UpdateComplaintStatusAsync(UpdateComplaintStatusDto dto, string modifiedByUserId)
    {
        var complaints = await _unitOfWork.Complaints.FindAsync(c => c.Id == dto.Id);
        var complaint = complaints?.FirstOrDefault();
        if (complaint == null) return false;

        var oldStatus = complaint.Status;
        complaint.Status = dto.Status;
        
        if (dto.Priority.HasValue)
        {
            complaint.Priority = dto.Priority.Value;
        }

        _unitOfWork.Complaints.Update(complaint);

        var log = new ComplaintLog
        {
            ComplaintId = complaint.Id,
            OldStatus = oldStatus,
            NewStatus = complaint.Status,
            ActionDetails = $"Status updated to {complaint.Status} by staff.",
            ModifiedByUserId = modifiedByUserId
        };
        await _unitOfWork.ComplaintLogs.AddAsync(log);

        await _unitOfWork.CompleteAsync();
        return true;
    }

    public async Task<bool> AssignDepartmentAsync(Guid complaintId, Guid departmentId, string modifiedByUserId)
    {
        var complaints = await _unitOfWork.Complaints.FindAsync(c => c.Id == complaintId);
        var complaint = complaints?.FirstOrDefault();
        if (complaint == null) return false;

        complaint.DepartmentId = departmentId;
        _unitOfWork.Complaints.Update(complaint);

        var dept = await _unitOfWork.Departments.GetByIdAsync(departmentId);
        var log = new ComplaintLog
        {
            ComplaintId = complaint.Id,
            ActionDetails = $"Assigned to department: {dept?.Name ?? "Unknown"}",
            ModifiedByUserId = modifiedByUserId
        };
        await _unitOfWork.ComplaintLogs.AddAsync(log);

        await _unitOfWork.CompleteAsync();
        return true;
    }
    
    private Priority CalculateSmartPriority(string categoryName)
    {
        // simplistic smart priority logic based on keywords
        var lower = categoryName.ToLower();
        if (lower.Contains("urgent") || lower.Contains("emergency")) return Priority.Critical;
        if (lower.Contains("bug") || lower.Contains("issue")) return Priority.High;
        if (lower.Contains("feedback")) return Priority.Low;
        return Priority.Medium;
    }
}
