using Application.Data.AuthorizationTokens;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoraWorkManager.Models;

namespace ShoraWorkManager.Areas.Identity.Pages.Account.Manage
{
    public class CreateAuthorizationTokensModel : PageModel
    {
        private readonly ILogger<CreateAuthorizationTokensModel> _logger;
        private readonly IMediator _mediator;

        public CreateAuthorizationTokensModel(ILogger<CreateAuthorizationTokensModel> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [BindProperty]
        public CreateAuthorizationTokenViewModel PageModel { get; set; } = new();

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadLastTokenAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostGenerateAsync()
        {
            var createResult = await _mediator.Send(new CreateAuthorizationToken.Query());

            if (!createResult.IsSuccess)
            {
                foreach(var error in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                _logger.LogWarning("Failed to create authorization token: {Error}", createResult.ToString());
                
                await LoadLastTokenAsync();
                return Page();
            }

            StatusMessage = "A new authorization token was generated successfully.";
            _logger.LogInformation("Authorization token generated with id {0}", createResult?.Value?.Id);

            await LoadLastTokenAsync();

            return RedirectToPage(); 
        }

        private async Task LoadLastTokenAsync()
        {
            var lastResult = await _mediator.Send(new GetLastAuthorizationToken.Query());

            if (lastResult.IsSuccess)
            {
                PageModel.LastToken = lastResult.Value.Token;
                return;
            }

            PageModel.LastToken = null;
        }
    }
}
