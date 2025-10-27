using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Persistence.Models;

namespace Application.Data.Account
{
    public class ChangeUserRole
    {
        public class Command : IRequest<Result<IdentityResult>>
        {
            public string UserId { get; set; }
            public string NewRole { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<IdentityResult>>
        {
            private readonly UserManager<User> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;
            private readonly ILogger<ChangeUserRole> _logger;

            public Handler(UserManager<User> userManager, ILogger<ChangeUserRole> logger, RoleManager<IdentityRole> roleManager)
            {
                _userManager = userManager;
                _logger = logger;
                _roleManager = roleManager;
            }

            public async Task<Result<IdentityResult>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(request.UserId);
                    if (user == null)
                    {
                        return Result<IdentityResult>.Failure("User not Found.");
                    }

                    if (string.IsNullOrEmpty(request.NewRole) || !await _roleManager.RoleExistsAsync(request.NewRole))
                    {
                        return Result<IdentityResult>.Failure("New Role not found.");
                    }

                    // Remove o user de todos os roles
                    var resultRemoveRole = await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                    if (!resultRemoveRole.Succeeded)
                    {
                        return Result<IdentityResult>.Failure("Error removing old role from the user");
                    }

                    // Adicionar o role selecionado ao utilizador
                    var resultAddRole = await _userManager.AddToRoleAsync(user, request.NewRole);
                    if (!resultAddRole.Succeeded)
                    {
                        return Result<IdentityResult>.Failure("Error adding new role to the user");
                    }

                    return Result<IdentityResult>.Success(resultAddRole);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while changing the user.");
                    return Result<IdentityResult>.Failure("Something wrong occured, try again.");
                }
            }
        }
    }
}
