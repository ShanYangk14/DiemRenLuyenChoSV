using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PMQLSV.Models;
using System.Security.Claims;

namespace PMQLSV.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "TeacherPolicy")]
    public class TeacherController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SchoolDbContext _db;
        public TeacherController(ILogger<HomeController> logger, SchoolDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult TeacherPage()
        {
            try
            {
                var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
                if (roles.Contains("Teacher"))
                {
                    _logger.LogInformation("User is authenticated. Access granted to Teacher page. User: {user}", User.Identity.Name);
                    var userEmail = User.Identity.Name;
                    var teacher = _db.Teachers
                        .Include(t => t.User)
                        .Include(t => t.Classes)
                        .FirstOrDefault(s => s.User.Email == userEmail);
					if (teacher == null)
                    {
                        _logger.LogWarning("Student information not found for user: {user}", userEmail);
                        return RedirectToAction("Error");
                    }
                    return View(teacher);
                }
                else
                {
                    _logger.LogWarning("Access to Teacher page denied. User not authenticated.");
                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the Student action.");
                return RedirectToAction("Error");
            }
        }
        public IActionResult ManageStudent()
        {
            try
            {
                var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
                if (roles.Contains("Teacher"))
                {
                    _logger.LogInformation("User is authenticated. Access granted to Manage Student page. User: {user}", User.Identity.Name);
                    var userEmail = User.Identity.Name;
                    var teacher = _db.Teachers
                        .Include(t => t.User)
                        .Include(t => t.Classes)
                        .FirstOrDefault(s => s.User.Email == userEmail);

                    if (teacher == null)
                    {
                        _logger.LogWarning("Teacher information not found for user: {user}", userEmail);
                        return RedirectToAction("Error");
                    }

                    // Get the IDs of the classes taught by the teacher
                    var classIds = teacher.Classes.Select(c => c.Id).ToList();

                    // Get the list of students for the classes taught by the teacher
                    var studentsManage = _db.Students
                        .Include(s => s.User)
                        .Include(s => s.Grades) // Include grades for each student
                        .Where(s => classIds.Contains(s.ClassId))
                        .ToList();

                    return View(studentsManage);
                }
                else
                {
                    _logger.LogWarning("Access to Manage Student page denied. User not authenticated.");
                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the ManageStudent action.");
                return RedirectToAction("Error");
            }
        }

        public IActionResult AddScore(int studentId)
        {
            try
            {
                // Find the student by ID
                var student = _db.Students
                    .Include(s => s.User)
                    .FirstOrDefault(s => s.Id == studentId);

                if (student == null)
                {
                    _logger.LogWarning("Student not found with ID: {studentId}", studentId);
                    return RedirectToAction("Error");
                }

                // Create a new instance of Grades for the student
                var grade = new Grades
                {
                    StudentId = studentId
                };

                // Populate ViewBag.StudentId with SelectListItems
                var studentsList = _db.Students.Select(s => new SelectListItem
                {
                    Text = s.Id.ToString(),
                    Value = s.Id.ToString()
                }).ToList();
                ViewBag.StudentId = new SelectList(studentsList, "Value", "Text", studentId);

                return View(grade);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the AddScore action.");
                return RedirectToAction("Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddScore(Grades grades, int studentId)
        {
            try
            {
                // Add logic to save grade to the database
                if (!ModelState.IsValid)
                {
                    // Assuming StudentId needs to be assigned here
                    grades.StudentId = studentId;

                    _db.Grades.Add(grades);
                    _db.SaveChanges();

                    return RedirectToAction("ManageStudent");
                }
                else
                {
                    // If model state is invalid, return to the view with validation errors
                    return View(grades);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                _logger.LogError(ex, "An error occurred in the AddScore action.");
                return RedirectToAction("Error");
            }
        }

        public IActionResult EditScore(int gradeId)
        {
            try
            {
                // Find the grade by ID
                var grade = _db.Grades.Find(gradeId);

                if (grade == null)
                {
                    _logger.LogWarning("Grade not found with ID: {gradeId}", gradeId);
                    return RedirectToAction("Error");
                }

                return View(grade);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the EditScore action.");
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        public IActionResult EditScore(Grades grade)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Update the grade in the database
                    _db.Grades.Update(grade);
                    _db.SaveChanges();

                    return RedirectToAction("ManageStudent");
                }

                return View(grade);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while editing score.");
                return RedirectToAction("Error");
            }
        }

    }
}
