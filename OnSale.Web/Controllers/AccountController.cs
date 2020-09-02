using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnSale.Common.Entities;
using OnSale.Common.Enums;
using OnSale.Common.Responses;
using OnSale.Web.Data;
using OnSale.Web.Data.Entities;
using OnSale.Web.Helpers;
using OnSale.Web.Models;
using System;
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
        private readonly IMailHelper _mailHelper;

        public AccountController(DataContext context, //inyectamos nuestras implementacion para poder dar manejo a nuestra vistas correctamente
        IUserHelper userHelper,
         ICombosHelper combosHelper,
        IBlobHelper blobHelper,
        IMailHelper mailHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _blobHelper = blobHelper;
            _mailHelper = mailHelper;
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

                string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);//Generamos el token de seguridad que vamos a enviar al email del nuevo user
                string tokenLink = Url.Action("ConfirmEmail", "Account", new //del controlador Account llamaremos a una accion llamada ConfirmEmail 
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                Response response = _mailHelper.SendMail(model.Username, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                    $"To allow the user, " +
                    $"plase click in this link:<p><a href = \"{tokenLink}\">Confirm Email</a></p>");
                if (response.IsSuccess)//Si la reapuesta es OK
                {
                    ViewBag.Message = "The instructions to allow your user has been sent to email.";//nos dira que fue enviado un correo
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, response.Message);

            }

            model.Countries = _combosHelper.GetComboCountries();
            model.Departments = _combosHelper.GetComboDepartments(model.CountryId);
            model.Cities = _combosHelper.GetComboCities(model.DepartmentId);
            return View(model);
        }
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            User user = await _userHelper.GetUserAsync(new Guid(userId));
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
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

        public async Task<IActionResult> ChangeUser()
        {
            User user = await _userHelper.GetUserAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }
            //Una busqueda hacia arriba el contrario del sentido de la relacion
            //Busqueme el departamentos donde las ciudades  de dicho departamento tenga por lo menos dicha ciudad
            Department department = await _context.Departments.FirstOrDefaultAsync(d => d.Cities.FirstOrDefault(c => c.Id == user.City.Id) != null);
            if (department == null)
            {
                department = await _context.Departments.FirstOrDefaultAsync();
            }
            //Una busqueda hacia arriba el contrario del sentido de la relacion , buscamos el paises donde cuyo departamento tenga dicho Id que buscamos
            Country country = await _context.Countries.FirstOrDefaultAsync(c => c.Departments.FirstOrDefault(d => d.Id == department.Id) != null);
            if (country == null)
            {
                country = await _context.Countries.FirstOrDefaultAsync();
            }

            EditUserViewModel model = new EditUserViewModel//Creamos una nueva vista con todos sus datos
            {
                Address = user.Address,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                ImageId = user.ImageId,
                Cities = _combosHelper.GetComboCities(department.Id),
                CityId = user.City.Id,
                Countries = _combosHelper.GetComboCountries(),
                CountryId = country.Id,
                DepartmentId = department.Id,
                Departments = _combosHelper.GetComboDepartments(country.Id),
                Id = user.Id,
                Document = user.Document
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = model.ImageId;//Creamos un Gui por si hay nueva imagen

                if (model.ImageFile != null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                }

                User user = await _userHelper.GetUserAsync(User.Identity.Name);

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                user.ImageId = imageId;
                user.City = await _context.Cities.FindAsync(model.CityId);
                user.Document = model.Document;

                await _userHelper.UpdateUserAsync(user);//Actualizamos el user
                return RedirectToAction("Index", "Home");//Y redireccionamos al index
            }

            model.Cities = _combosHelper.GetComboCities(model.DepartmentId);//En caso de fallos armamos los combos nuevamente
            model.Countries = _combosHelper.GetComboCountries();
            model.Departments = _combosHelper.GetComboDepartments(model.CityId);
            return View(model);
        }
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserAsync(User.Identity.Name);
                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ChangeUser");//Si lo pudo cambiar lo retornamos a la vista ChangeUser
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);//Mostramos modal con error  
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User no found.");
                }
            }

            return View(model);
        }
    }

}
