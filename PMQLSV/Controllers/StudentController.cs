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

                    // Retrieve student information along with associated class information based on the user's email
                    var userEmail = User.Identity.Name;
                    var student = _db.Students
                        .Include(s => s.User)
                        .Include(s => s.Class) 
                        .FirstOrDefault(s => s.User.Email == userEmail);

                    if (student == null)
                    {
                        _logger.LogWarning("Student information not found for user: {user}", userEmail);
                        return RedirectToAction("Error");
                    }

                    return View(student); // Pass the student model to the view
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

        public IActionResult ScoresAndReviews(int studentId, int Grade)
        {
            try
            {
                // Fetch the student information, including related grades
                var student = _db.Students
                    .Include(s => s.User)
                    .Include(s => s.Grades)
                    .FirstOrDefault(s => s.Id == studentId);

                if (student != null && student.Grades != null && student.Grades.Any())
                {
                    // Log the event
                    string fullName = student.User != null ? $"{student.User.FirstName} {student.User.LastName}" : "Unknown";
                    ViewBag.Message = $"Displaying scores and reviews for student: {fullName}";

                    return View(student.Grades); // Pass the grades object to the view
                }
                else
                {
                    // Log the event
                    ViewBag.Message = $"No grade information available for student with ID {studentId}.";

                    // Return the ScoresAndReviews view with no grade information
                    return View(Enumerable.Empty<Grades>()); // Pass an empty list of grades to the view
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
    

