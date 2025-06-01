using Ckc.EShop.Infrastructure.Identity;
using Ckc.EShop.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ckc.EShop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly string _externalCookieScheme;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _externalCookieScheme = IdentityConstants.ExternalScheme;        
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(_externalCookieScheme);
            ViewData["ReturnUrl"] = returnUrl;
            return View();        
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email,
                    model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login Attempt.");
                    return View(model);
                }
            }
             return View(model);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(CatalogController.Index), "Catalog");

            }
        }
    }
}
