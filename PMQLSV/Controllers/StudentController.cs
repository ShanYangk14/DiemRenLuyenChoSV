using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMQLSV.Models;
using System.Linq;
using System.Security.Claims;

namespace PMQLSV.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "StudentPolicy")]
    public class StudentController : Controller
    {
        private readonly ILogger<StudentController> _logger;
        private readonly SchoolDbContext _db;

        public StudentController(ILogger<StudentController> logger, SchoolDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult StudentPage()
        {
            try
            {
                var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
                if (roles.Contains("Student"))
                {
                    _logger.LogInformation("User is authenticated. Access granted to Student page. User: {user}", User.Identity.Name);

                    // Retrieve student information along with associated class information based on the user's email
                    var userEmail = User.Identity.Name;
                    var student = _db.Students
                        .Include(s => s.User)
                        .Include(s => s.Class)
                            .ThenInclude(c => c.Teacher)
                            .ThenInclude(t => t.User)
                        .FirstOrDefault(s => s.User.Email == userEmail);

                    if (student == null)
                    {
                        _logger.LogWarning("Student information not found for user: {user}", userEmail);
                        return RedirectToAction("Error");
                    }

                    return View(student); // Display student information
                }
                else
                {
                    _logger.LogWarning("Access to Student page denied. User not authenticated.");
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the StudentPage action.");
                return RedirectToAction("Error");
            }
        }

        [HttpGet]
        public IActionResult ScoresAndReviews()
        {
            try
            {
                var userEmail = User.Identity.Name;
                var student = _db.Students
                    .Include(s => s.User)
                    .Include(s => s.Class)
                        .ThenInclude(c => c.Teacher)
                    .Include(s => s.Grades) // Include grades related to the student
                    .FirstOrDefault(s => s.User.Email == userEmail);

                if (student == null)
                {
                    _logger.LogWarning("Student information not found for user: {user}", userEmail);
                    return RedirectToAction("Error");
                }

                return View(student.Grades); // Pass grades associated with the student to the view
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the ScoresAndReviews action.");
                return RedirectToAction("Error");
            }
        }
    }
}
