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

        [HttpGet]
        public IActionResult SendCode() => View();

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token, string userId)
        {
            var result = await _accountService.CheckIfTokenResetPasswordIsUsed(userId);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View("Error");
            }

            ViewBag.Token = token;
            ViewBag.UserId = userId;

            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult MyAccount() => View();

        [Authorize]
        [HttpGet]
        public IActionResult UpdatePassword() => View();

        //--------------------------------------------------------------------------------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var (result, user) = await _accountService.CreateUser(model.Email, model.Password);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.SendCode(model.Email);

                if (result.Success)
                {
                    ViewBag.ShowSuccessMessage = true;
                    return View();
                }

                ModelState.AddModelError(string.Empty, result.Message);
                return View();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.ResetPassword(model.UserId, model.Token, model.NewPassword);
                if (result.Success)
                {
                    ViewBag.SuccessMessage = true;
                    return View();
                }

                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }
            ViewBag.UserId = model.UserId;
            ViewBag.Token = model.Token;
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.UpdatePassword(model.NewPassword, model.CurrPassword, User);
                if (result.Success)
                {
                    TempData["MessageSuccess"] = "Senha atualizada com sucesso.";
                    return RedirectToAction(nameof(MyAccount));
                }

                ModelState.AddModelError(string.Empty, result.Message);
                return View();
            }

            return View(model);
        }

        //-------------------------------------------------------------------------------------------------------
    }
}
