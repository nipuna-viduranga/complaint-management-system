using ComplaintSystem.Application.Interfaces;
using ComplaintSystem.Domain.Enums;
using ComplaintSystem.Domain.Interfaces;

namespace ComplaintSystem.Application.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IUnitOfWork _unitOfWork;

    public AnalyticsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Dictionary<ComplaintStatus, int>> GetComplaintsByStatusAsync()
    {
        var complaints = await _unitOfWork.Complaints.GetAllAsync();
        return complaints.GroupBy(c => c.Status).ToDictionary(g => g.Key, g => g.Count());
    }

    public async Task<int> GetTotalComplaintsCountAsync()
    {
        var complaints = await _unitOfWork.Complaints.GetAllAsync();
        return complaints.Count();
    }

    public async Task<Dictionary<string, int>> GetComplaintsByCategoryAsync()
    {
        var complaints = await _unitOfWork.Complaints.GetAllAsync();
        var categories = await _unitOfWork.Categories.GetAllAsync();
        
        var dict = new Dictionary<string, int>();
        foreach (var category in categories)
        {
            dict[category.Name] = complaints.Count(c => c.CategoryId == category.Id);
        }
        return dict;
    }
}
