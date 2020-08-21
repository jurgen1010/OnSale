using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnSale.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnSale.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly DataContext _context;

        public CategoriesController(DataContext context)
        {
            _context = context;//Traemos el contexto de la base de datos
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());   //Retornamos la lista de categorias a la vista
        }
    }

}
