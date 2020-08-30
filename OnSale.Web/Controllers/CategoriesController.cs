using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnSale.Common.Entities;
using OnSale.Web.Data;
using OnSale.Web.Helpers;
using OnSale.Web.Models;
using System;
using System.Threading.Tasks;

namespace OnSale.Web.Controllers
{

    [Authorize(Roles = "Admin")]//Solo tendra acceso el administrador a este controlador
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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            //A la vista no le pasamos el model Category sino que le pasamos el CategoryViewModel haciendo la conversion con la implementacion ToCategoryViewModel
            CategoryViewModel model = _converterHelper.ToCategoryViewModel(category);
            return View(model);
        }

        [HttpPost]// El usuario a traves del POST modifica la categoria
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = model.ImageId;//Guardamos la imagen

                if (model.ImageFile != null)//Si hay una nueva imagen la subimos al blob
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "categories");
                }

                try
                {
                    //Ahora viceversa convertimos el model a Category para poderlo llevar a la Db, con parametro el false porque no estamos creando la categoria sino editandola
                    Category category = _converterHelper.ToCategory(model, imageId, false);
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
