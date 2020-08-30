using Microsoft.AspNetCore.Identity;
using OnSale.Web.Data.Entities;
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

    }
}
