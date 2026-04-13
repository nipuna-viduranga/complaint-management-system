using ComplaintSystem.Domain.Enums;
using ComplaintSystem.Domain.Interfaces;

namespace ComplaintSystem.Application.Interfaces;

public interface IAnalyticsService
{
    Task<Dictionary<ComplaintStatus, int>> GetComplaintsByStatusAsync();
    Task<int> GetTotalComplaintsCountAsync();
    Task<Dictionary<string, int>> GetComplaintsByCategoryAsync();
}
