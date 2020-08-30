using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnSale.Web.Data.Entities;
using OnSale.Web.Helpers;
using OnSale.Web.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OnSale.Web.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;

        public AccountController(IUserHelper userHelper, IConfiguration configuration)
        {
            _userHelper = userHelper;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("CreateToken")]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)//Indicamos que el cuerpo que vamos a recibir es un model LoginViewModel
        {
            if (ModelState.IsValid)//validamos el modelo
            {
                User user = await _userHelper.GetUserAsync(model.Username);
                if (user != null)
                {
                    Microsoft.AspNetCore.Identity.SignInResult result = await _userHelper.ValidatePasswordAsync(user, model.Password);//Validamos que el user y el password coinciden

                    if (result.Succeeded)// si es exitoso el resultado
                    {
                        Claim[] claims = new[]
                        {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Email),//Creamos una coleccion de claims con email y un Gui pra que sea unico para cada token
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));//Obtenemos la llave del token de nuestro appsetting.json y la encryptamos
                        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);//encriptamos las credenciales con el algoritmo HmacSha256

                        JwtSecurityToken token = new JwtSecurityToken(
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims, //Generamos un nuevo token con los claims
                            expires: DateTime.UtcNow.AddDays(99),//Con una vigencia de 99 dias  
                            signingCredentials: credentials);
                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),// como resultado tengo mi token, la expiracion y el user
                            expiration = token.ValidTo,
                            user
                        };

                        return Created(string.Empty, results);//Retornamos los resultados
                    }
                }
            }

            return BadRequest();//sino es valido el modelo devolvemos un bad request
        }
    }

}
