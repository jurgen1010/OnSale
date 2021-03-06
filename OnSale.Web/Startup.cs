﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OnSale.Web.Data;
using OnSale.Web.Data.Entities;
using OnSale.Web.Helpers;
using System.Globalization;
using System.Text;

namespace OnSale.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/NotAuthorized";//Si se encuentra algun error en el cual no se encuentra el camino direccionamos a la vista NotAuthorized
                options.AccessDeniedPath = "/Account/NotAuthorized";//Tambien si se quiere acceder a una ruta no autorizada
            });



            services.AddIdentity<User, IdentityRole>(cfg =>
            {
                cfg.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
                cfg.SignIn.RequireConfirmedEmail = true;//Le indicamos a la aplicacion al iniciar que al registrarse un user requiere confirmacion de email
                cfg.User.RequireUniqueEmail = true;
                cfg.Password.RequireDigit = false;
                cfg.Password.RequiredUniqueChars = 0;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequireUppercase = false;
            }).AddDefaultTokenProviders()//Agregamos la configuracion del token que se enviara a los correos de cada usuario que se registre en la page
            .AddEntityFrameworkStores<DataContext>();//Por ultimo le indicamos a nuestro servicio que va trabajar mediante nuestro DataContext

            //Inyectamos la conexion a la base de datos
            services.AddDbContext<DataContext>(cfg =>
            {
                cfg.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            //Inyectamos nuestra autenticacion atraves de token
            services.AddAuthentication()
               .AddCookie()
               .AddJwtBearer(cfg =>
               {
                   cfg.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidIssuer = Configuration["Tokens:Issuer"],
                       ValidAudience = Configuration["Tokens:Audience"],
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))
                   };
               });

            services.AddTransient<SeedDb>();//Inyectamos el SeedDb, alimentador de la base de datos
            services.AddScoped<IBlobHelper, BlobHelper>(); //Inyectamos la configuracion al blobStorage (al llamar una instancia de IBlobHelper, retornara un BlobHelper)
            services.AddScoped<IConverterHelper, ConverterHelper>();//Inyectamos el convertidor de CategoryViewModel <=> Category (al llamar una instancia de IConverterHelper, retornara un ConverterHelper)
            services.AddScoped<ICombosHelper, CombosHelper>();
            services.AddScoped<IUserHelper, UserHelper>();//Iyectamos nuestro helper desde donde vamos administrar nuestros usuarios
            services.AddScoped<IMailHelper, MailHelper>();//Iyectamos nuestro helper desde donde vamos administrar el envio de correos con token de confirmacion
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseStatusCodePagesWithReExecute("/error/{0}");//Cuando no encontramos una pagina enviaremos el error 404
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();//Indicamos a  nuestra configuracion que nuestra aplicacion usara autenticacion
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
