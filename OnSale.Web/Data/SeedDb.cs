using OnSale.Common.Entities;
using OnSale.Common.Enums;
using OnSale.Web.Data.Entities;
using OnSale.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnSale.Web.Data
{
    public class SeedDb
    {

        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();//Si la base de datos no esta creada automaticamente se creara
            await CheckCountriesAsync();//Validara que exista paises en la Db
            await CheckRolesAsync();//Chekea los roles
            await CheckUserAsync("1010", "Jurgen", "Perez", "jurgen1010@hotmail.com", "318 557 0824", "Calle Del Olvido ", UserType.Admin);

        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());//Asi no exitan los roles se crea un roll administrador
            await _userHelper.CheckRoleAsync(UserType.User.ToString());//Crea un roll usuario
        }

        private async Task<User> CheckUserAsync(
            string document,
            string firstName,
            string lastName,
            string email,
            string phone,
            string address,
            UserType userType)
        {
            User user = await _userHelper.GetUserAsync(email);//Checkea si el user existe
            if (user == null)
            {
                user = new User
                {
                    FirstName = firstName,//Me crea el user con los parametros que le indique en la linea 27
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    City = _context.Cities.FirstOrDefault(),
                    UserType = userType
                };

                await _userHelper.AddUserAsync(user, "123456");//Me crea el user con la clave 123456 en caso de que no exista
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());//Adicionamos el user al roll administrador
            }

            return user;
        }


        private async Task CheckCountriesAsync()
        {
            if (!_context.Countries.Any())//Preguntamos si la Db contiene paises
            {
                _context.Countries.Add(new Country // creamos paises con departamentos y a su vez ciudades
                {
                    Name = "Colombia",
                    Departments = new List<Department>
                {
                    new Department
                    {
                        Name = "Antioquia",
                        Cities = new List<City>
                        {
                            new City { Name = "Medellín" },
                            new City { Name = "Envigado" },
                            new City { Name = "Itagüí" }
                        }
                    },
                    new Department
                    {
                        Name = "Bogotá",
                        Cities = new List<City>
                        {
                            new City { Name = "Bogotá" }
                        }
                    },
                    new Department
                    {
                        Name = "Valle del Cauca",
                        Cities = new List<City>
                        {
                            new City { Name = "Calí" },
                            new City { Name = "Buenaventura" },
                            new City { Name = "Palmira" }
                        }
                    }
                }
                });
                _context.Countries.Add(new Country
                {
                    Name = "USA",
                    Departments = new List<Department>
                {
                    new Department
                    {
                        Name = "California",
                        Cities = new List<City>
                        {
                            new City { Name = "Los Angeles" },
                            new City { Name = "San Diego" },
                            new City { Name = "San Francisco" }
                        }
                    },
                    new Department
                    {
                        Name = "Illinois",
                        Cities = new List<City>
                        {
                            new City { Name = "Chicago" },
                            new City { Name = "Springfield" }
                        }
                    }
                }
                });
                await _context.SaveChangesAsync();//Guardamos los paises en la Db
            }
        }
    }
}
