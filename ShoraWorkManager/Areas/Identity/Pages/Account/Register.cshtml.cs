// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoraWorkManager.Models;

namespace ShoraWorkManager.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IMediator _mediator;

        public RegisterModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty]
        public RegisterViewModel InputViewModel { get; set; }
        public string ReturnUrl { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                var result = await _mediator.Send(new Application.Data.Account.RegisterAccount.Command
                {
                    FirstName = InputViewModel.FirstName,
                    LastName = InputViewModel.LastName,
                    Email = InputViewModel.Email,
                    Password = InputViewModel.Password,
                    ConfirmPassword = InputViewModel.ConfirmPassword
                });

                if (result.IsSuccess)
                {
                    return LocalRedirect(returnUrl);
                }

                foreach ( string error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

       
    }
}
