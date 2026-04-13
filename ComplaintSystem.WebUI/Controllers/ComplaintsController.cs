using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using ComplaintSystem.Application.Interfaces;
using ComplaintSystem.Application.DTOs;
using ComplaintSystem.Domain.Interfaces;
using ComplaintSystem.Infrastructure.Data;
using ComplaintSystem.Domain.Enums;

namespace ComplaintSystem.WebUI.Controllers;

[Authorize]
public class ComplaintsController : Controller
{
    private readonly IComplaintService _complaintService;
    private readonly ICommentService _commentService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly Microsoft.AspNetCore.SignalR.IHubContext<Hubs.NotificationHub> _hubContext;

    public ComplaintsController(
        IComplaintService complaintService,
        ICommentService commentService,
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        Microsoft.AspNetCore.SignalR.IHubContext<Hubs.NotificationHub> hubContext)
    {
        _complaintService = complaintService;
        _commentService = commentService;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _hubContext = hubContext;
    }

    // LIST for normal users
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var complaints = await _complaintService.GetComplaintsByUserIdAsync(userId);
        return View(complaints);
    }

    // LIST for Admin/Staff
    [Authorize(Roles = "Admin,Staff,Supervisor")]
    public async Task<IActionResult> AdminIndex(ComplaintStatus? status)
    {
        var complaints = await _complaintService.GetAllComplaintsAsync(status);
        ViewBag.CurrentStatus = status;
        return View(complaints);
    }

    // CREATE
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _unitOfWork.Categories.GetAllAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateComplaintDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _unitOfWork.Categories.GetAllAsync();
            return View(dto);
        }

        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        await _complaintService.CreateComplaintAsync(dto, userId);
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"New complaint filed: {dto.Title}");
        return RedirectToAction(nameof(Index));
    }

    // DETAILS
    public async Task<IActionResult> Details(Guid id)
    {
        var complaint = await _complaintService.GetComplaintByIdAsync(id);
        if (complaint == null) return NotFound();

        // Check authorization
        var userId = _userManager.GetUserId(User);
        if (!User.IsInRole("Admin") && !User.IsInRole("Staff") && complaint.UserId != userId)
        {
            return Forbid();
        }

        var comments = await _commentService.GetCommentsByComplaintIdAsync(id, User.IsInRole("Admin") || User.IsInRole("Staff"));
        var logs = await _unitOfWork.ComplaintLogs.FindAsync(l => l.ComplaintId == id);
        
        ViewBag.Comments = comments;
        ViewBag.Logs = logs.OrderByDescending(l => l.CreatedAt);
        ViewBag.Departments = await _unitOfWork.Departments.GetAllAsync();

        return View(complaint);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Staff,Supervisor")]
    public async Task<IActionResult> UpdateStatus(UpdateComplaintStatusDto dto)
    {
        var userId = _userManager.GetUserId(User);
        await _complaintService.UpdateComplaintStatusAsync(dto, userId!);
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"Complaint status updated to {dto.Status}");
        return RedirectToAction(nameof(Details), new { id = dto.Id });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Staff,Supervisor")]
    public async Task<IActionResult> AssignDepartment(Guid id, Guid departmentId)
    {
        var userId = _userManager.GetUserId(User);
        await _complaintService.AssignDepartmentAsync(id, departmentId, userId!);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(CreateCommentDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Content)) return BadRequest();

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        await _commentService.AddCommentAsync(dto, user.Id, user.FullName);
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"New comment on complaint by {user.FullName}");
        return RedirectToAction(nameof(Details), new { id = dto.ComplaintId });
    }
}
