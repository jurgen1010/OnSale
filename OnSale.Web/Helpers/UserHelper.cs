using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnSale.Common.Enums;
using OnSale.Web.Data;
using OnSale.Web.Data.Entities;
using OnSale.Web.Models;
using System;
using System.Threading.Tasks;

namespace OnSale.Web.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;

        //Basicamente esta clase nos esta empaquetando el manejo de user y roles desde una sola y no tener que inyectar nuestra implementacion desde clase separadas

        //Inyectamos en DataContext, para admin User inyectamos UserManager en la tabla User y para la Admin de roles los hacemos con RoleManager en la tabla Role
        public UserHelper(DataContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);//Llamamos a clase UserManager para usar el metodo que nos permite crear el usuario
        }

        public async Task<User> AddUserAsync(AddUserViewModel model, Guid imageId, UserType userType)
        {
            User user = new User
            {
                Address = model.Address,
                Document = model.Document,
                Email = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                ImageId = imageId,
                PhoneNumber = model.PhoneNumber,
                City = await _context.Cities.FindAsync(model.CityId),//Encontramos la ciudad por medio del Id que nos trae la vista AddUserViewModel
                UserName = model.Username,
                UserType = userType
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);//Creamos el User
            if (result != IdentityResult.Success)//Validamos que si haya sido posible la creacion
            {
                return null;
            }

            User newUser = await GetUserAsync(model.Username);//Obtenemos el user
            await AddUserToRoleAsync(newUser, user.UserType.ToString());//Luego le creamos un roll a este user
            return newUser;//Retornemos le nuevo usuario

        }

        public async Task AddUserToRoleAsync(User user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);//Adicionamos el roll
        }

        public async Task CheckRoleAsync(string roleName)
        {
            bool roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)//validamos que el roll exista
            {
                await _roleManager.CreateAsync(new IdentityRole //Creamos un nuevoroll con su nombre
                {
                    Name = roleName
                });
            }
        }

        public async Task<User> GetUserAsync(string email)
        {
            return await _context.Users //Vaya la coleccion de usuarios
                .Include(u => u.City)//Traiga la ciudad
                .FirstOrDefaultAsync(u => u.Email == email);// por medio del email que estoy facilitando
        }

        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(// le pasamos
                model.Username,
                model.Password,
                model.RememberMe,
                false);//Desactivamos los bloqueos de intectos de logueo que son 3
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<SignInResult> ValidatePasswordAsync(User user, string password)
        {
            return await _signInManager.CheckPasswordSignInAsync(user, password, false);

        }
        public async Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            return await _context.Users
                .Include(u => u.City)
                .FirstOrDefaultAsync(u => u.Id == userId.ToString());
        }
        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }


    }

}
