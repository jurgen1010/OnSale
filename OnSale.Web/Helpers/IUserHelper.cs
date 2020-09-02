using Microsoft.AspNetCore.Identity;
using OnSale.Common.Enums;
using OnSale.Web.Data.Entities;
using OnSale.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnSale.Web.Helpers
{
    public interface IUserHelper
    {
        //Le entrengo el email y me retorna el user
        Task<User> GetUserAsync(string email);

        //Para agregar users por medio de user y password
        Task<IdentityResult> AddUserAsync(User user, string password);

        //Para verificar el roll del user
        Task CheckRoleAsync(string roleName);

        //Agregar user a un roll
        Task AddUserToRoleAsync(User user, string roleName);

        //validar si se enrolo
        Task<bool> IsUserInRoleAsync(User user, string roleName);
        Task<SignInResult> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
        Task<SignInResult> ValidatePasswordAsync(User user, string password);//Para validar si es valido el user
        Task<User> AddUserAsync(AddUserViewModel model, Guid imageId, UserType userType);
        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);
        Task<IdentityResult> UpdateUserAsync(User user);
        Task<User> GetUserAsync(Guid userId);
        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);






    }
}
