using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnSale.Common.Entities;
using OnSale.Common.Enums;
using OnSale.Web.Data;
using OnSale.Web.Data.Entities;
using OnSale.Web.Helpers;
using OnSale.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnSale.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IBlobHelper _blobHelper;


        public AccountController(DataContext context, //inyectamos nuestras implementacion para poder dar manejo a nuestra vistas correctamente
        IUserHelper userHelper,
         ICombosHelper combosHelper,
        IBlobHelper blobHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _blobHelper = blobHelper;

        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)//Preguntamos si se pudo loguear
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))//Validamos que la URL tenga direccion de retorno
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());//Redirecciona a la direccion de retorno
                    }

                    return RedirectToAction("Index", "Home");//Sino va al index 
                }

                ModelState.AddModelError(string.Empty, "Email or password incorrect.");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult NotAuthorized()
        {
            return View();
        }
        public IActionResult Register()
        {
            AddUserViewModel model = new AddUserViewModel//Creamos una nueva instancia de la vista
            {
                Countries = _combosHelper.GetComboCountries(),// en el combo de countries recupero todos los paises
                Departments = _combosHelper.GetComboDepartments(0),//enviamos los departamentos del pais 0, aparecera en blanco hasta el user no me seleccione un pais
                Cities = _combosHelper.GetComboCities(0),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;//Creamos un Gui vacio porque no se sabe si el model tiene imagen

                if (model.ImageFile != null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");//Subimos la imagen 
                }

                User user = await _userHelper.AddUserAsync(model, imageId, UserType.User);//Creamos un nuevo user de de tipo user
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "This email is already used.");//Si el user es repetido enviamos el mensaje a traves de un modal de error
                    model.Countries = _combosHelper.GetComboCountries();//Armamamos los combos de nuevo antes de volver a la vista para que no se pierdan
                    model.Departments = _combosHelper.GetComboDepartments(model.CountryId);
                    model.Cities = _combosHelper.GetComboCities(model.DepartmentId);
                    return View(model);
                }

                LoginViewModel loginViewModel = new LoginViewModel//Creamos las nuevas credenciales de logueo
                {
                    Password = model.Password,
                    RememberMe = false,
                    Username = model.Username
                };

                var result2 = await _userHelper.LoginAsync(loginViewModel);//Nos logueamos automaticamente despues de registrarnos

                if (result2.Succeeded)
                {
                    return RedirectToAction("Index", "Home");//Si se logueo redireccionamos el Home
                }
            }

            model.Countries = _combosHelper.GetComboCountries();
            model.Departments = _combosHelper.GetComboDepartments(model.CountryId);
            model.Cities = _combosHelper.GetComboCities(model.DepartmentId);
            return View(model);
        }


        public JsonResult GetDepartments(int countryId)//JsonResult para el llamado en AJAX dropdownlist en cascada para no recargar la pagina solamente los datos
        {
            Country country = _context.Countries
                .Include(c => c.Departments)
                .FirstOrDefault(c => c.Id == countryId);
            if (country == null)
            {
                return null;
            }

            return Json(country.Departments.OrderBy(d => d.Name));//Retornamos todos los departamentos en formato Json
        }

        public JsonResult GetCities(int departmentId)
        {
            Department department = _context.Departments
                .Include(d => d.Cities)
                .FirstOrDefault(d => d.Id == departmentId);
            if (department == null)
            {
                return null;
            }

            return Json(department.Cities.OrderBy(c => c.Name));//Retornamos todos las ciudades en formato Json
        }

    }

}
