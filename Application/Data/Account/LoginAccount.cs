using Application.Core;
using Domain.Exceptions;
using Domain.Validators;
using Domain.Validators.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Persistence.Models;

namespace Application.Data.Account
{
    public class LoginAccount
    {
        public class Command : IRequest<Result<SignInResult>>
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public bool RememberMe { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<SignInResult>>
        {
            private readonly SignInManager<User> _signInManager;
            private readonly ILogger<LoginAccount> _logger;

            public Handler(SignInManager<User> signInManager, ILogger<LoginAccount> logger)
            {
                _signInManager = signInManager;
                _logger = logger;
            }

            public async Task<Result<SignInResult>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    IDataValidator<string> userValidator = new EmailValidator();

                    try
                    {
                        userValidator.Validate(request.Email);
                    }
                    catch (CustomValidationException ex)
                    {
                        return Result<SignInResult>.Failure(ex.ErrorsMessage);
                    }

                    var user = await _signInManager.UserManager.FindByEmailAsync(request.Email);

                    if (user == null)
                    {
                        return Result<SignInResult>.Failure("Invalid login attempt.");
                    }

                    var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, request.RememberMe, lockoutOnFailure: false);


                    if(result.Succeeded)
                    {
                        _logger.LogInformation("User logged in.");
                        return Result<SignInResult>.Success(result);
                    }

                    return Result<SignInResult>.Failure("Invalid login attempt.");

                }
                catch (Exception ex)
                {
                    return Result<SignInResult>.Failure("Something wrong occured, try again.");
                }
            }
        }
    }
}
