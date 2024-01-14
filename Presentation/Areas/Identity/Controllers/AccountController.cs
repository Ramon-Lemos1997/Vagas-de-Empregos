using Domain.Entites.User;
using Domain.Interfaces.Identity;
using Domain.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace Presentation.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly IAccountInterface _accountService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, IAccountInterface accountService)
        {
            _signInManager = signInManager;
            _accountService = accountService;
        }


        //---------------------------------------------------------------------------------------------------------------

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AccessDenied() => View();

        //--------------------------------------------------------------------------------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var (result, user) = await _accountService.CreateUserAsync(model.Email, model.Password);

                if (result.Success)
                {
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    return RedirectToAction("Index", "Home", new { area = "" });
                }

                ModelState.AddModelError(string.Empty, result.Message);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home", new { area = "" });
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Você excedeu o número de tentativas, aguarde 1 hora e tente novamente.");
                    return View();
                }

                ModelState.AddModelError(string.Empty, "Falha na autenticação. Verifique seu email/senha e tente novamente.");
            }
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { area = "" });
        }




        //-------------------------------------------------------------------------------------------------------
    }
}
