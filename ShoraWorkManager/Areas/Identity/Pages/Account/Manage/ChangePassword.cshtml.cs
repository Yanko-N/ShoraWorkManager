// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoraWorkManager.Models;

namespace ShoraWorkManager.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IMediator _mediator;

        public ChangePasswordModel( IMediator mediator,UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _mediator = mediator;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public ChangePasswordViewModel Input { get; set; }


        [TempData]
        public string StatusMessage { get; set; }

      
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword) //If there is no password we must sign out this user
            {
                await _signInManager.SignOutAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var result = await _mediator.Send(new Application.Data.Account.ChangePassword.Command()
            {
                OldPassword = Input.OldPassword,
                NewPassword = Input.NewPassword,
                UserClaims = User
            });

            if(!result.IsSuccess)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);

            StatusMessage = "Your password has been changed.";

            return RedirectToPage();
        }
    }
}
