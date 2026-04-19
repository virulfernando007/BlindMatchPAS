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
        public async Task<IActionResult> Users()
        {
            
            var users = await _userManager.Users.ToListAsync();
            
            
            
            var userRows = new List<AdminUserRow>(); 
            return View(userRows);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View(new RegisterViewModel());
        }

        [HttpGet]
        public IActionResult MigrationHistory()
        {
            
            return View(); 
        }
    }
}