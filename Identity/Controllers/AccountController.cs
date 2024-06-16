using Identity.Models;
using Identity.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using UserMangmentService.Service;

namespace Identity.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailServices _emailService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailServices emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }
        public IActionResult Register()
        {
            RegisterViewModel registerViewModel = new();
            return View(registerViewModel);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                Error(result);
            }
            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }
        public IActionResult Login()
        {
            LoginViewModel loginViewModel = new();
            return View(loginViewModel);

        }
        [HttpGet]
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
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // لا تكشف أن المستخدم غير موجود أو لم يتم تأكيد البريد الإلكتروني
                    return RedirectToAction("ForgotPasswordConfirmation");
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var urlEncoder = UrlEncoder.Default;
                var encodedToken = urlEncoder.Encode(token);
                var ForgotPasswordlink = $"http://localhost:7080/resetPassword?token={token}&email={user.Email}";

                var message = new UserMangmentService.Models.Message(new string[] { user.Email! }, "Reset Password", ForgotPasswordlink!);

                _emailService.SendEmail(message);
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        public void Error (IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
