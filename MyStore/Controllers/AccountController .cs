using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyStore.Data;
using MyStore.Model;
using MyStore.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyStore.Controllers
{
    /// <summary>
    /// Manages user authentication, registration, and account-related actions.
    /// Includes methods for login, registration, password change, and logout.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IPasswordHasher<User> _hasher;

        public AccountController(ApplicationDbContext db,
                                 IPasswordHasher<User> hasher)
        {
            _db = db;
            _hasher = hasher;
        }

        [HttpGet]
        public IActionResult Register()
        {
            // Retrieve all roles from the database to be displayed in the registration form
            var roles = _db.Roles.ToList(); 
            var availableRoles = roles.Select(role => new SelectListItem
            {
                Value = role.Name, 
                Text = role.Name  
            }).ToList();

            var registerViewModel = new RegisterViewModel
            {
                AvailableRoles = availableRoles 
            };

            return View(registerViewModel);
        }


        [HttpPost]
        public IActionResult Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AvailableRoles = _db.Roles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToList();

                return View(vm);
            }
            // Check if the email is already in use
            if (_db.Users.Any(u => u.Email == vm.Email))
            {
                ModelState.AddModelError("", "Email already in use");

                // Re-populate AvailableRoles before returning the view
                vm.AvailableRoles = _db.Roles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToList();

                return View(vm);
            }

            var user = new User
            {
                Email = vm.Email,
                FullName = vm.FullName,
                Address = vm.Address,
                DateOfBirth = vm.DateOfBirth
            };
            user.PasswordHash = _hasher.HashPassword(user, vm.Password);

            _db.Users.Add(user);
            _db.SaveChanges();

            // Add the selected roles to the user
            if (vm.SelectedRoles != null)
            {
                foreach (var roleName in vm.SelectedRoles)
                {
                    var role = _db.Roles.SingleOrDefault(r => r.Name == roleName);
                    if (role != null)
                    {
                        _db.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
                    }
                }
            }

            _db.SaveChanges();

            // Auto-login the user
            SignIn(user);

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            // Retrieve the user from the database based on the entered email
            var user = _db.Users.SingleOrDefault(u => u.Email == vm.Email);
            if (user == null ||
                _hasher.VerifyHashedPassword(user, user.PasswordHash, vm.Password)
                    != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View(vm);
            }
            // If credentials are valid, sign in the user
            SignIn(user);
            return RedirectToAction("Index", "Products");

        }

        private void SignIn(User user)
        {
            var roles = _db.UserRoles
                           .Where(ur => ur.UserId == user.Id)
                           .Select(ur => ur.Role.Name)
                           .ToList();

            // Create a list of claims for the user (including roles)
            var claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName)
        };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            // Create a claims identity with the claims
            var ci = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Sign in the user by creating an authentication ticket in the cookie
            HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(ci));
        }

        [HttpPost]
        public IActionResult Logout()
        {
            // Sign out the user and clear the authentication cookie
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

       
    }
}

