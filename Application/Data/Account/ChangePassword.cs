using Application.Core;
using Domain.Exceptions;
using Domain.Validators;
using Domain.Validators.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Persistence.Models;
using System.Security.Claims;

namespace Application.Data.Account
{
    public class ChangePassword
    {
        public class Command : IRequest<Result<IdentityResult>>
        {
            public string NewPassword { get; set; }
            public string OldPassword { get; set; }
            public ClaimsPrincipal UserClaims { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<IdentityResult>>
        {
            private readonly UserManager<User> _userManager;
            private readonly ILogger<ChangePassword> _logger;

            public Handler(UserManager<User> userManager, ILogger<ChangePassword> logger)
            {
                _userManager = userManager;
                _logger = logger;
            }

            public async Task<Result<IdentityResult>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    IDataValidator<string> passwordValidator = new PasswordValidator();

                    try
                    {
                        passwordValidator.Validate(request.NewPassword);
                    }
                    catch (CustomValidationException ex)
                    {
                        return Result<IdentityResult>.Failure(ex.ErrorsMessage);
                    }

                    var user = await _userManager.GetUserAsync(request.UserClaims);

                    if (user == null)
                    {
                        return Result<IdentityResult>.Failure("User not found.");
                    }

                    var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User password was changed with sucess.");
                        return Result<IdentityResult>.Success(result);
                    }

                    return Result<IdentityResult>.Failure(result.Errors.Select(x=>x.Description));

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while changing password.");
                    return Result<IdentityResult>.Failure("Something wrong occured, try again.");
                }
            }
        }
    }
}
}
