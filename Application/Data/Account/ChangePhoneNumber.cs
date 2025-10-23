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
    public class ChangePhoneNumber
    {
        public class Command : IRequest<Result<IdentityResult>>
        {
            public string NewPhoneNumber { get; set; }
            public string OldPhoneNumber { get; set; }
            public ClaimsPrincipal UserClaims { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<IdentityResult>>
        {
            private readonly UserManager<User> _userManager;
            private readonly ILogger<ChangePhoneNumber> _logger;

            public Handler(UserManager<User> userManager, ILogger<ChangePhoneNumber> logger)
            {
                _userManager = userManager;
                _logger = logger;
            }

            public async Task<Result<IdentityResult>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if(request.OldPhoneNumber == request.NewPhoneNumber)
                    {
                        return Result<IdentityResult>.Failure("The new PhoneNumber must be different from the old PhoneNumber.");
                    }

                    IDataValidator<string> phoneNumberValidator = new PhoneNumberValidator();

                    try
                    {
                        phoneNumberValidator.Validate(request.NewPhoneNumber);
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

                    var result = await _userManager.SetPhoneNumberAsync(user,request.NewPhoneNumber);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User PhoneNumber was changed with sucess.");
                        return Result<IdentityResult>.Success(result);
                    }

                    return Result<IdentityResult>.Failure(result.Errors.Select(x => x.Description));

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while changing PhoneNumber.");
                    return Result<IdentityResult>.Failure("Something wrong occured, try again.");
                }
            }
        }
    }
}
