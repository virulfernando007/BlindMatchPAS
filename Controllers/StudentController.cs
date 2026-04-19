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

            var project = await _projectService.GetProjectForEditAsync(user.Id, id);
            if (project == null) return NotFound();

            if (project.Status == ProjectStatus.Matched)
            {
                TempData["Error"] = "A matched project cannot be edited.";
                return RedirectToAction(nameof(Dashboard));
            }

            if (project.Status == ProjectStatus.Withdrawn)
            {
                TempData["Error"] = "A withdrawn project cannot be edited.";
                return RedirectToAction(nameof(Dashboard));
            }

            var areas = await _researchAreaRepo.GetActiveAreasAsync();

            var selectedIds = string.IsNullOrEmpty(project.ResearchAreaIds)
                ? new List<int>()
                : project.ResearchAreaIds
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => int.TryParse(x.Trim(), out var n) ? n : 0)
                    .Where(x => x > 0)
                    .ToList();

            return View(new ProjectSubmissionViewModel
            {
                Id                      = id,
                Title                   = project.Title,
                ShortDescription        = project.ShortDescription,
                Abstract                = project.Abstract,
                SelectedResearchAreaIds = selectedIds,
                ResearchAreas           = areas,
                AttachmentPath          = project.AttachmentPath
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int projectId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var dashboard = await _projectService.GetStudentDashboardAsync(user.Id, user.FullName);
            var row = dashboard.Projects.FirstOrDefault(p => p.ProjectId == projectId);

            if (row == null)
            {
                TempData["Error"] = "Project not found.";
                return RedirectToAction(nameof(Dashboard));
            }

            if (row.Status != ProjectStatus.Withdrawn)
            {
                TempData["Error"] = "Only withdrawn projects can be deleted.";
                return RedirectToAction(nameof(Dashboard));
            }

            await _projectService.DeleteProjectAsync(user.Id, projectId);

            TempData["Success"] = "Project deleted permanently.";
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            return View(new StudentProfileViewModel
            {
                FullName   = user.FullName,
                Email      = user.Email ?? "",
                StudentId  = user.StudentId ?? "",
                Batch      = user.Batch ?? "",
                Faculty    = user.Faculty ?? "",
                DegreeName = user.DegreeName ?? "",
                University = user.University ?? "",
                CreatedAt  = user.CreatedAt
            });
        }

        public Task<IActionResult> Profile(StudentProfileViewModel model)
        {
            return Profile(model, ModelState);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(StudentProfileViewModel model, Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            user.FullName  = model.FullName;
            user.StudentId = model.StudentId;
            user.Batch     = model.Batch;
            user.Faculty   = model.Faculty;
            user.DegreeName = model.DegreeName;
            user.University = model.University;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Profile updated successfully.";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
                modelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }    
    }   

}