using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlindMatchPAS.Models;
using BlindMatchPAS.ViewModels;
using BlindMatchPAS.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace BlindMatchPAS.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.TotalStudents = (await _userManager.GetUsersInRoleAsync("Student")).Count;
            ViewBag.TotalSupervisors = (await _userManager.GetUsersInRoleAsync("Supervisor")).Count;
            ViewBag.TotalLeaders = (await _userManager.GetUsersInRoleAsync("ModuleLeader")).Count;
            ViewBag.TotalAdmins = (await _userManager.GetUsersInRoleAsync("Admin")).Count;
            
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Users(string roleFilter, bool? statusFilter)
        {
            var allUsers = await _userManager.Users.ToListAsync();
            var users = new List<AdminUserRow>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                users.Add(new AdminUserRow
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? string.Empty,
                    Role = roles.FirstOrDefault() ?? "No Role",
                    IsActive = !user.LockoutEnabled,
                    CreatedAt = user.CreatedAt
                });
            }

            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string userId, string fullName, string department, bool isActive)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.FullName = fullName;
            user.Department = department;
            user.IsActive = isActive;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "User updated successfully.";
                return RedirectToAction(nameof(Users));
            }

            ModelState.AddModelError("", "Failed to update user.");
            return View(user);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View(new RegisterViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> MigrationHistory()
        {
            try
            {
                var migrations = await _context.Database.SqlQueryRaw<MigrationHistory>(
                    "SELECT MigrationId, ProductVersion, AppliedAt FROM __EFMigrationsHistory ORDER BY AppliedAt DESC"
                ).ToListAsync();
                return View(migrations);
            }
            catch
            {
                return View(new List<MigrationHistory>());
            }
        }
    }
}