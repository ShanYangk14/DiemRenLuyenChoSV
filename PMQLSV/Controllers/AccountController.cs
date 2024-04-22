using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PMQLSV.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Text;

namespace PMQLSV.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly SchoolDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly SchoolDbContext _context;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public AccountController(ILogger<AccountController> logger, SchoolDbContext db, UserManager<User> userManager, SignInManager<User> signInManager, SchoolDbContext context, RoleManager<IdentityRole<int>> roleManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
            CheckRoleManager();
        }
        public void CheckRoleManager()
        {
            if (!_roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityRole<int> role = new IdentityRole<int>();
                role.Name = "Admin";
                IdentityResult roleResult = _roleManager.CreateAsync(role).Result;
            }
            if (!_roleManager.RoleExistsAsync("Teacher").Result)
            {
                IdentityRole<int> role = new IdentityRole<int>();
                role.Name = "Teacher";
                IdentityResult roleResult = _roleManager.CreateAsync(role).Result;
            }
            if (!_roleManager.RoleExistsAsync("Student").Result)
            {
                IdentityRole<int> role = new IdentityRole<int>();
                role.Name = "Student";
                IdentityResult roleResult = _roleManager.CreateAsync(role).Result;
            }
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        public ActionResult Register()
        {
            ViewBag.Classes = _db.Classes.ToList();
            return View();
        }


        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.FullName = HttpContext.Session.GetString("FullName");
                ViewBag.Email = HttpContext.Session.GetString("Email");
                return View("Index");
            }
            else
            {
                return View();
            }
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterAsync(User _user, bool TeacherRole, bool StudentRole, bool AdminRole, string ClassName, int ClassId, int? ClassSize, string Major)
        {
            try
            {
                var check = await _userManager.FindByEmailAsync(_user.Email);

                if (check == null)
                {
                    var user = new User
                    {
                        FirstName = _user.FirstName,
                        LastName = _user.LastName,
                        Email = _user.Email,
                        Password = GetMD5(_user.Password),
                        ResetToken = string.Empty,
                        ResetTokenExpiration = DateTime.UtcNow,
                        EmailConfirmationToken = Guid.NewGuid().ToString(),
                        SecurityStamp = Guid.NewGuid().ToString()
                    };

                    if (TeacherRole)
                    {
                        user.Role = new Role { rolename = "Teacher" };
                    }
                    else if (StudentRole)
                    {
                        user.Role = new Role { rolename = "Student" };
                    }
                    else if (AdminRole)
                    {
                        user.Role = new Role { rolename = "Admin" };
                    }

                    _db.Users.Add(user);
                    _db.SaveChanges();

                    if (TeacherRole)
                    {
                        var newTeacher = new Teacher
                        {
                            UserId = user.Id // Use the ID of the newly created user
                        };
                        _db.Teachers.Add(newTeacher);
                        _db.SaveChanges();

                        // Create a new class and assign it to the teacher
                        var newClass = new Class
                        {
                            ClassName = ClassName,
                            ClassSize = ClassSize.Value,
                            TeacherId = newTeacher.Id
                        };
                        _db.Classes.Add(newClass);
                        _db.SaveChanges();
                    }
                    else if (StudentRole)
                    {
                        var selectedClass = await _db.Classes.FirstOrDefaultAsync(c => c.Id == ClassId);
                        if (selectedClass != null)
                        {
                            // Create a new Student object
                            var newStudent = new Student
                            {
                                UserId = user.Id, // Assign the user ID of the newly created user
                                Major = Major,
                                ClassId = selectedClass.Id // Assign the retrieved class ID
                            };

                            // Add the new student to the Students table
                            _db.Students.Add(newStudent);

                            // Save changes to the database
                             _db.SaveChanges();
                        }
                        else
                        {
                            // Handle case where the selected class is not found
                            ViewBag.error = "Selected class not found.";
                            return View();
                        }
                    }
                    else if (AdminRole)
                    {
                        user.Role = new Role { rolename = "Admin" };
                    }

                    // Assign roles based on the selected checkboxes
                    if (user.Role != null)
                    {
                        await _userManager.AddToRoleAsync(user, user.Role.rolename);
                    }

                    return RedirectToAction("AdminPage", "Admin");
                }
                else
                {
                    ViewBag.error = "Email already exists";
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration.");
                ViewBag.error = "An error occurred during registration.";
                return View();
            }
        }

        private string DetermineRoleForUser(User user)
        {
            var role = _db.Roles.FirstOrDefaultAsync(s => s.Id == user.RoleId).Result;
            if (role != null)
            {
                if (role.rolename.ToString().Equals("Teacher", StringComparison.OrdinalIgnoreCase))
                {
                    return "Teacher";
                }
                else if (role.rolename.ToString().Equals("Student", StringComparison.OrdinalIgnoreCase))
                {
                    return "Student";
                }
                else if (role.rolename.ToString().Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    return "Admin";
                }
            }
          
            return "Student";
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(string email, string password)
        {
            if (ModelState.IsValid)
            {
                string hashedPassword = GetMD5(password);
                var user = await _db.Users.FirstOrDefaultAsync(s => s.Email == email && s.Password == hashedPassword);

                if (user != null)
                {
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Email),
                        };

                    // Add custom role claim based on user's role
                    string role = DetermineRoleForUser(user);
                    claims.Add(new Claim(ClaimTypes.Role, role));

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    // Redirect to appropriate action based on role
                    if (role == "Teacher")
                    {
                        return RedirectToAction("TeacherPage", "Teacher");
                    }
                    else if (role == "Student")
                    {
                        return RedirectToAction("StudentPage", "Student");
                    }
                    else if (role == "Admin")
                    {
                        return RedirectToAction("AdminPage", "Admin");
                    }
                }
                else
                {
                    ViewBag.error = "Login failed";
                    return RedirectToAction(nameof(Login));
                }
            }

            return View();
        }
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        public IActionResult InvalidToken()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);

            if (user != null)
            {
                user.ResetToken = Guid.NewGuid().ToString();
                user.ResetTokenExpiration = DateTime.UtcNow.AddHours(1);

                _db.SaveChanges();

                return RedirectToAction("ResetPassword", new { token = user.ResetToken });
            }

            ViewBag.Error = "Email address not found.";
            return View("ForgotPassword");
        }
         public IActionResult ResetPassword(string token)
        {
            var user = _db.Users.FirstOrDefault(u => u.ResetToken == token && u.ResetTokenExpiration > DateTime.UtcNow);

            if (user != null)
            {
                return View(new ResetPasswordViewModel { Token = token });
            }

            return RedirectToAction("InvalidToken");
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            var user = _db.Users.FirstOrDefault(u => u.ResetToken == model.Token && u.ResetTokenExpiration > DateTime.UtcNow);

            if (user != null)
            {

                user.Password = GetMD5(model.NewPassword);
                user.ConfirmPassword = GetMD5(model.NewPassword);
                user.ResetToken = string.Empty;
                user.ResetTokenExpiration = DateTime.UtcNow;

                _db.SaveChanges();

                return RedirectToAction("Login");
            }

            return RedirectToAction("InvalidToken");
        }
    }
}
