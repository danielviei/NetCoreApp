using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using UserNamespace.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using ApplicationDbContextNamespace.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace UserNamespace.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<CustomUser> _userManager;

        private readonly SignInManager<CustomUser> _signInManager;

        private readonly IWebHostEnvironment _hostingEnvironment;

        private readonly IEmailSender<CustomUser> _emailSender;


        private readonly ApplicationDbContext _context;

        public UserController(
            IWebHostEnvironment hostingEnvironment,
            UserManager<CustomUser> userManager,
            SignInManager<CustomUser> signInManager,
            ApplicationDbContext context,
            IEmailSender<CustomUser> emailSender
        )
        {
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailSender = emailSender;

        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            var model = new LoginViewModel
            {
                Email = "",
                Password = ""
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Aquí puedes establecer los errores de validación en el ViewModel
                model.EmailError = "Email is required.";
                model.PasswordError = "Password is required.";
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // Verifica si el usuario está bloqueado
                if (user.IsBlocked)
                {
                    model.EmailError = "This account has been blocked.";
                    model.PasswordError = "This account has been blocked.";
                    return View(model);
                }

                var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())); // Fix: Convert user.Id to string
                identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)); // Fix: Use null conditional operator to handle possible null value

                identity.AddClaim(new Claim("IsStaff", user.IsStaff.ToString().ToLower()));
                await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(identity));

                return RedirectToAction("Index", "Home");
            }

            model.EmailError = "Invalid login attempt.";
            model.PasswordError = "Invalid login attempt.";
            return View(model);
        }

        public IActionResult Register()
        {
            var model = new RegisterViewModel();
            ViewData["Title"] = "Register";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var filePath = Path.GetTempFileName();
                string ImagePath = "";
                if (model.Img != null)
                {
                    // Define a unique name for the file
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Img.FileName);

                    // Define the path to the folder where you want to save the images
                    var imagesFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");

                    // If the folder doesn't exist, create it
                    if (!Directory.Exists(imagesFolder))
                    {
                        Directory.CreateDirectory(imagesFolder);
                    }

                    // Define the full path to the file
                    filePath = Path.Combine(imagesFolder, fileName);

                    // Save the file
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await model.Img.CopyToAsync(stream);
                    }

                    // Save the relative path to the file in the database
                    ImagePath = "/images/" + fileName;
                }

                var CustomUser = new CustomUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Name = model.Name,
                    Lastname = model.Lastname,
                    ImagePath = ImagePath,
                };

                if (string.IsNullOrEmpty(model.Password))
                {
                    throw new ArgumentException("Password cannot be empty.");
                }

                var result = await _userManager.CreateAsync(CustomUser);

                if (result.Succeeded)
                {
                    // User created successfully, now set the password
                    var token = await _userManager.GeneratePasswordResetTokenAsync(CustomUser);
                    var passwordResult = await _userManager.ResetPasswordAsync(CustomUser, token, model.Password);

                    if (!passwordResult.Succeeded)
                    {
                        foreach (var error in passwordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }

                    return RedirectToAction("Login");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Profile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
            {
                return NotFound();
            }

            var user = _context.Users.Find(userId);

            var model = new ProfileViewModel();
            if (user == null)
            {
                return NotFound();
            }
            model.Lastname = user.Lastname;
            model.Name = user.Name;
            model.Email = user.Email;
            model.ImagePath = user.ImagePath;
            model.Id = user.Id;

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var user = await _context.Users.FindAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                // Actualizar el modelo de usuario aquí
                // Si se proporciona ImageFile, procesarlo y actualizar el campo Img del modelo

                if (model.ImageFile != null)
                {
                    // Define a unique name for the file
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);

                    // Define the path to the folder where you want to save the images
                    var imagesFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");

                    // If the folder doesn't exist, create it
                    if (!Directory.Exists(imagesFolder))
                    {
                        Directory.CreateDirectory(imagesFolder);
                    }

                    // Define the full path to the file
                    var filePath = Path.Combine(imagesFolder, fileName);

                    // Save the file
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    // Save the relative path to the file in the database
                    string ImagePath = "/images/" + fileName;
                    user.ImagePath = ImagePath;
                }

                user.Name = model.Name;
                user.Lastname = model.Lastname;
                user.Email = model.Email;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return Redirect("/User/Profile");
            }

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult PasswordReset()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PasswordReset(PasswordResetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("Profile");
        }

        // Acción para mostrar el formulario de recuperación de contraseña
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null
                    // || !(await _userManager.IsEmailConfirmedAsync(user))
                    )
                {
                    // No revelar que el usuario no existe o que no está confirmado
                    return View("ResetPassword");
                }

                // Generar el token para el restablecimiento de contraseña
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "User", new { email = user.Email, token = token }, protocol: HttpContext.Request.Scheme);

                // Enviar el correo electrónico
                await _emailSender.SendEmailAsync(model.Email, "Reset Password", $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

                return View("ResetPasswordConfirmation");
            }

            return View(model);
        }

        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.ErrorMessage);
                    }
                }
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                // No revelar que el usuario no existe
                return View("ResetPasswordConfirmation");
            }
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "The password and confirmation password do not match.");
                return View(model);
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password ?? "");

            if (result.Succeeded)
            {
                // Redirigir al usuario a la página de inicio de sesión después de un restablecimiento de contraseña exitoso
                return RedirectToAction("Login");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        private bool UserExists(int id)
        {
            return _context.Publications.Any(e => e.Id == id);
        }
    }
}