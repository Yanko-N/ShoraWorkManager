using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoraWorkManager.Models;

namespace ShoraWorkManager.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly SignInManager<Persistence.Models.User> _signInManager;
        private readonly UserManager<Persistence.Models.User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMediator _mediator;


        private List<SelectListItem> AvaiableRoles => _roleManager.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();

        public AccountController(SignInManager<Persistence.Models.User> signInManager, UserManager<Persistence.Models.User> userManager,
            RoleManager<IdentityRole> roleManager,IMediator mediator)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _mediator = mediator;
        }

        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = AppConstants.Roles.ADMIN)]
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users;
            var model = new List<UsersRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                model.Add(new UsersRolesViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = roles.SingleOrDefault()
                });
            }

            return View(model);
        }

        [Authorize(Roles = AppConstants.Roles.ADMIN)]
        public async Task<IActionResult> Editar(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            var model = new UsersRolesViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
                Roles = AvaiableRoles
            };

            return View(model);
        }

        [Authorize(Roles = AppConstants.Roles.ADMIN)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(string id, UsersRolesViewModel model)
        {
            if (!ModelState.IsValid)
            {

                model.Roles = AvaiableRoles;
                model.Email = (await _userManager.FindByIdAsync(id))?.Email;
                return View(model);
            }
            
            var result = await _mediator.Send(new Application.Data.Account.ChangeUserRole.Command
            {
                UserId = id,
                NewRole = model.Role
            });

            if (result.IsFailure)
            {
                result.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error);
                });

                model.Roles = AvaiableRoles;
                model.Email = (await _userManager.FindByIdAsync(id))?.Email;
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
