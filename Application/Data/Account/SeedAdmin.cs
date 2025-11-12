using Application.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Persistence.Models;

namespace Application.Data.Account
{
    public class SeedAdmin
    {
        public static void Seed(IUserStore<User> userStore,IUserEmailStore<User> emailStore,
            UserManager<User> userManager,IConfiguration configuration)
        {

            var userTask = userManager.FindByEmailAsync("admin@admin.pt");

            userTask.Wait();

            if (userTask.Result != null)
            {
                return;
            }

            string? secretPassword = configuration["Account:AdminPassword"];
            string? secretEmail = configuration["Account:AdminEmail"];
            if (string.IsNullOrWhiteSpace(secretPassword) || string.IsNullOrWhiteSpace(secretEmail))
            {
                throw new Exception("Admin Account Must be seeded, so the password and email is necessary");
            }

            User? user = Activator.CreateInstance<User>();

            if(user == null)
            {
                throw new Exception("Admin Account Must be seeded");
            }

           
            user.FirstName = "Admin";
            user.LastName = "Account";
            user.Email = secretEmail;

            var username = $"{user.FirstName}-{user.LastName}-{Guid.NewGuid()}";

            var setUserNameTask = userStore.SetUserNameAsync(user, username, CancellationToken.None);
            var emailStoreTask = emailStore.SetEmailAsync(user, user.Email, CancellationToken.None);

            setUserNameTask.Wait();
            emailStoreTask.Wait();

            var resultTask = userManager.CreateAsync(user,secretPassword);

            resultTask.Wait();

            if (!resultTask.Result.Succeeded)
            {
                throw new Exception("Admin Account Must be seeded");
            }

            var roleTask = userManager.AddToRoleAsync(user, AppConstants.Roles.ADMIN);

            roleTask.Wait();
        }
    }
}
