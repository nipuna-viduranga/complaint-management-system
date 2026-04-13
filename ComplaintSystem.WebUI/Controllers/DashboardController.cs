using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ComplaintSystem.Application.Interfaces;

namespace ComplaintSystem.WebUI.Controllers;

[Authorize(Roles = "Admin,Staff,Supervisor")]
public class DashboardController : Controller
{
    private readonly IAnalyticsService _analyticsService;

    public DashboardController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    public async Task<IActionResult> Index()
    {
        var totalCount = await _analyticsService.GetTotalComplaintsCountAsync();
        var statusStats = await _analyticsService.GetComplaintsByStatusAsync();
        var categoryStats = await _analyticsService.GetComplaintsByCategoryAsync();

        ViewBag.TotalCount = totalCount;
        ViewBag.StatusStats = statusStats;
        ViewBag.CategoryStats = categoryStats;

        return View();
    }
}
