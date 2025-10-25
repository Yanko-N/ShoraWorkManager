using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ShoraWorkManager.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public readonly SignInManager<Persistence.Models.User> _signInManager;

        public AccountController(SignInManager<Persistence.Models.User> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
