using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnSale.Common.Entities;
using OnSale.Web.Data;
using OnSale.Web.Helpers;
using OnSale.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnSale.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly DataContext _context; //Empiezan con underLine porque son atributos privados
        private readonly IConverterHelper _converterHelper;
        private readonly IBlobHelper _blobHelper;

        //Ademas de la config de la base de datos, inyectamos las config del blobHelper y converterHelper
        public CategoriesController(DataContext context, IBlobHelper blobHelper, IConverterHelper converterHelper)
        {
            _context = context;//Traemos el contexto de la base de datos
            _blobHelper = blobHelper;
            _converterHelper = converterHelper;          
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());   //Retornamos la lista de categorias a la vista
        }
        public IActionResult Create()
        {
            CategoryViewModel model = new CategoryViewModel();//Retornamos un CategoryViewModel porque viene con Imagen
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty; 

                if (model.ImageFile != null)// Si el usuario seleccion imagen 
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "categories");//Le indicamos suba la imagen seleccionada en el folder categories
                }

                try
                {
                    //Pero convertimos esa imagen que el usuario subio a Category ya que mi Dd no Admite un dato tipo CategoryViewModel
                    Category category = _converterHelper.ToCategory(model, imageId, true); //Convertidomo el model usando el imageId y true para indicar que es un imagen nueva
                    _context.Add(category);//Adicionamos la categoria
                    await _context.SaveChangesAsync();//Guardamos cambios
                    return RedirectToAction(nameof(Index));//Retornamos al index
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "There are a record with the same name.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            return View(model);
        }


    }

}
