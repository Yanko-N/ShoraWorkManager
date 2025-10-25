using Application.Core;
using Application.Data.AuthorizationTokens;
using Domain.Exceptions;
using Domain.Validators;
using Domain.Validators.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Persistence.Models;

namespace Application.Data.Account
{
    public class RegisterAccount
    {
        public class Command : IRequest<Result<User>>
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string ConfirmPassword { get; set; }
            public string AuthorizationToken { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<User>>
        {
            private readonly SignInManager<User> _signInManager;
            private readonly UserManager<User> _userManager;
            private readonly IUserStore<User> _userStore;
            private readonly IUserEmailStore<User> _emailStore;
            private readonly ILogger<RegisterAccount> _logger;
            private readonly IMediator _mediator;

            public Handler(UserManager<User> userManager, IUserStore<User> userStore,
                SignInManager<User> signInManager, ILogger<RegisterAccount> logger,
                IMediator mediator)
            {
                _userManager = userManager;
                _userStore = userStore;
                _emailStore = GetEmailStore();
                _signInManager = signInManager;
                _logger = logger;
                _mediator = mediator;
            }

            public async Task<Result<User>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if (request.Password != request.ConfirmPassword)
                    {
                        return Result<User>.Failure("Password and Confirm Password do not match.");
                    }

                    IDataValidator<UserValidator.UserValidatorInput> userValidator = new UserValidator();

                    try
                    {
                        userValidator.Validate(new UserValidator.UserValidatorInput
                        {
                            FirstName = request.FirstName,
                            LastName = request.LastName,
                            Email = request.Email,
                            Password = request.Password
                        });
                    }
                    catch (CustomValidationException ex)
                    {
                        return Result<User>.Failure(ex.ErrorsMessage);
                    }

                    Result<AuthorizationToken> lastToken = await _mediator.Send(new GetLastAuthorizationToken.Query());

                    if(lastToken.IsFailure)
                    {
                        return Result<User>.Failure(lastToken.Errors);
                    }

                    if(string.IsNullOrWhiteSpace(lastToken?.Value?.Token ?? string.Empty))
                    {
                        return Result<User>.Failure("No valid authorization token found.");
                    }

                    if (lastToken?.Value?.Token != request.AuthorizationToken)
                    {
                        return Result<User>.Failure("Invalid authorization token.");
                    }

                    var user = CreateUser();

                    var username = $"{request.FirstName}-{request.LastName}-{Guid.NewGuid()}";

                    await _userStore.SetUserNameAsync(user, username, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, request.Email, CancellationToken.None);

                    user.FirstName = request.FirstName;
                    user.LastName = request.LastName;


                    var result = await _userManager.CreateAsync(user, request.Password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, AppConstants.Roles.ADMIN);

                        _logger.LogInformation("User created a new account with password.");
                        var userId = await _userManager.GetUserIdAsync(user);

                        await _signInManager.SignInAsync(user, isPersistent: false);

                        return Result<User>.Success(user);
                    }

                    List<string> errors = result.Errors
                        .Select(e => e.Description)
                        .ToList();

                    return Result<User>.Failure(errors);
                }
                catch (Exception ex)
                {
                    return Result<User>.Failure("Something wrong occured, try again.");
                }
            }

            private User CreateUser()
            {
                try
                {
                    return Activator.CreateInstance<User>();
                }
                catch
                {
                    throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                        $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                        $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
                }
            }


            private IUserEmailStore<User> GetEmailStore()
            {
                if (!_userManager.SupportsUserEmail)
                {
                    throw new NotSupportedException("The default UI requires a user store with email support.");
                }
                return (IUserEmailStore<User>)_userStore;
            }
        }
    }
}
