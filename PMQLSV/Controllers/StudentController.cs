using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMQLSV.Models;
using System.Security.Claims;

namespace PMQLSV.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "StudentPolicy")]
    public class StudentController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SchoolDbContext _db;
        public StudentController(ILogger<HomeController> logger, SchoolDbContext db)
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
                    return View();
                }
                else
                {
                    _logger.LogWarning("Access to Student page denied. User not authenticated.");
                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the Student action.");
                return RedirectToAction("Error");
            }
        }
        public IActionResult ScoresAndReviews(int studentId)
        {
            try
            {
                // Fetch the student information, including related grades
                var student = _db.Students
                    .Include(s => s.Grades)
                    .FirstOrDefault(s => s.Id == studentId);

                if (student != null)
                {
                    // Log the event
                    var fullName = $"{student.User.FirstName} {student.User.LastName}";
                    ViewBag.Message = $"Displaying scores and reviews for student: {fullName}";

                    // Pass the student object to the view
                    return View(student);
                }
                else
                {
                    // Log the event
                    ViewBag.Message = $"Student with ID {studentId} not found.";

                    // Redirect to a suitable action if student not found
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                // Log the error
                ViewBag.Message = "An error occurred while displaying scores and reviews for student.";

                // Redirect to an error page
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
    

