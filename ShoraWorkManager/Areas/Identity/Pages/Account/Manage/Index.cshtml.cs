// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Persistence.Models;
using ShoraWorkManager.Models;

namespace ShoraWorkManager.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMediator _mediator;
        public IndexModel(IMediator mediator,UserManager<User> userManager
            ,SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mediator = mediator;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public AccountIndexViewModel Input { get; set; }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new AccountIndexViewModel
            {
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            var result = await _mediator.Send(new Application.Data.Account.ChangePhoneNumber.Command
            {
                UserClaims = User,
                NewPhoneNumber = Input.PhoneNumber,
                OldPhoneNumber = phoneNumber
            });

            if (!result.IsSuccess)
            {
                foreach (string error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);

            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
