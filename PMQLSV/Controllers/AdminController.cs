using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMQLSV.Models;

namespace PMQLSV.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly SchoolDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly SchoolDbContext _context;
        public AdminController(ILogger<AdminController> logger, SchoolDbContext db, UserManager<User> userManager, SignInManager<User> signInManager, SchoolDbContext context) 
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public IActionResult AdminPage()
        {
            return View();
        }

        public IActionResult StudentsManagement()
        {
            var studentsManage = _db.Students
            .Include(s => s.User) // Include user information
            .Include(s => s.Grades)
            .ToList();

            return View(studentsManage);
        }

        public IActionResult TeachersManagement()
        {
            var teachersManage = _db.Teachers
                .Include(s => s.User)// Include user information
                .ToList();
            return View(teachersManage);
        }
    }
}
