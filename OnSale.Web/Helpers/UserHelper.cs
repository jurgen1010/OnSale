using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnSale.Web.Data;
using OnSale.Web.Data.Entities;
using System.Threading.Tasks;

namespace OnSale.Web.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        //Basicamente esta clase nos esta empaquetando el manejo de user y roles desde una sola y no tener que inyectar nuestra implementacion desde clase separadas

        //Inyectamos en DataContext, para admin User inyectamos UserManager en la tabla User y para la Admin de roles los hacemos con RoleManager en la tabla Role
        public UserHelper(DataContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);//Llamamos a clase UserManager para usar el metodo que nos permite crear el usuario
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


    }

}
