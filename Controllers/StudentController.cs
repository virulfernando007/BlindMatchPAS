using BlindMatchPAS.Models;
using BlindMatchPAS.Models.ViewModels;
using BlindMatchPAS.Repositories;
using BlindMatchPAS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlindMatchPAS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IResearchAreaRepository _researchAreaRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<StudentController> _logger;

        public StudentController(
            IProjectService projectService,
            IResearchAreaRepository researchAreaRepo,
            UserManager<ApplicationUser> userManager,
            ILogger<StudentController> logger)
        {
            _projectService   = projectService;
            _researchAreaRepo = researchAreaRepo;
            _userManager      = userManager;
            _logger           = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();
            var vm = await _projectService.GetStudentDashboardAsync(user.Id, user.FullName);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Submit()
        {
            var areas = await _researchAreaRepo.GetActiveAreasAsync();
            return View(new ProjectSubmissionViewModel { ResearchAreas = areas });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(ProjectSubmissionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ResearchAreas = await _researchAreaRepo.GetActiveAreasAsync();
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            string? attachmentPath = null;
            if (model.AttachmentFile != null && model.AttachmentFile.Length > 0)
            {
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".zip", ".png", ".jpg" };
                var extension = Path.GetExtension(model.AttachmentFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("AttachmentFile", "Only PDF, Word, ZIP, and image files are allowed.");
                    model.ResearchAreas = await _researchAreaRepo.GetActiveAreasAsync();
                    return View(model);
                }

                if (model.AttachmentFile.Length > 10 * 1024 * 1024)
                {
                    ModelState.AddModelError("AttachmentFile", "File size must not exceed 10MB.");
                    model.ResearchAreas = await _researchAreaRepo.GetActiveAreasAsync();
                    return View(model);
                }

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = $"{user.Id}_{DateTime.UtcNow.Ticks}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await model.AttachmentFile.CopyToAsync(stream);

                attachmentPath = $"/uploads/{uniqueFileName}";
            }

            var projectId = await _projectService.SubmitProjectAsync(user.Id, model, attachmentPath);
            _logger.LogInformation("Student {UserId} submitted project {ProjectId}", user.Id, projectId);

            TempData["Success"] = "Your project proposal has been submitted successfully!";
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var dashboard = await _projectService.GetStudentDashboardAsync(user.Id, user.FullName);
            var row = dashboard.Projects.FirstOrDefault(p => p.ProjectId == id);

            if (row == null) return NotFound();

            if (row.Status == ProjectStatus.Matched)
            {
                TempData["Error"] = "A matched project cannot be edited.";
                return RedirectToAction(nameof(Dashboard));
            }

            if (row.Status == ProjectStatus.Withdrawn)
            {
                TempData["Error"] = "A withdrawn project cannot be edited.";
                return RedirectToAction(nameof(Dashboard));
            }

            var areas = await _researchAreaRepo.GetActiveAreasAsync();
            return View(new ProjectSubmissionViewModel
            {
                Id            = id,
                Title         = row.Title,
                ResearchAreas = areas
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProjectSubmissionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ResearchAreas = await _researchAreaRepo.GetActiveAreasAsync();
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var success = await _projectService.UpdateProjectAsync(user.Id, model);

            if (!success)
            {
                TempData["Error"] = "Unable to update this project. It may already be matched.";
                return RedirectToAction(nameof(Dashboard));
            }

            TempData["Success"] = "Project updated successfully.";
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(int projectId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var success = await _projectService.WithdrawProjectAsync(user.Id, projectId);

            TempData[success ? "Success" : "Error"] = success
                ? "Project withdrawn successfully."
                : "Unable to withdraw this project. It may already be matched.";

            return RedirectToAction(nameof(Dashboard));
        }
    }
}