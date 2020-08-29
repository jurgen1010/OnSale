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
    public class ProductsController : Controller
    {
        private readonly DataContext _context;
        private readonly IBlobHelper _blobHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IConverterHelper _converterHelper;

        //Inyectamos nuestras implementaciones de configuracion de Db, IBlobHelper, ICombosHelper y IConverterHelper
        public ProductsController(DataContext context, IBlobHelper blobHelper, ICombosHelper combosHelper, IConverterHelper converterHelper)
        {
            _context = context;
            _blobHelper = blobHelper;
            _combosHelper = combosHelper;
            _converterHelper = converterHelper;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Products //Traigame de la Db los productos , incluyame la categoria y tambien las imagenes (INNER JOIN)
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .ToListAsync());
        }
        public IActionResult Create()
        {
            ProductViewModel model = new ProductViewModel
            {
                Categories = _combosHelper.GetComboCategories(),//Creeme un producto pero cargueme el comboCategories para que el user las pueda seleccionar
                IsActive = true //Asumo que el producto esta activo ya que lo estoy creando lo ponemos true por defecto
            };

            return View(model);//Retornamos al userel comboCategorias para que seleccione una
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)//Validamos que el modelo sea valido
            {
                try
                {
                    Product product = await _converterHelper.ToProductAsync(model, true);//convertimos el ProductViewModel a Product para poderlo salvar en la Db

                    if (model.ImageFile != null)
                    {
                        Guid imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "products");//Subimos la imagenes en el blob dentro de la coleccion de products
                        product.ProductImages = new List<ProductImage>//A las imagenes del producto le creamos una nueva lista de ProductImage
                {
                    new ProductImage { ImageId = imageId }//Le indicamos que la primer imagen del producto va ser igual al imageId que se almaceno en el blob
                };
                    }

                    _context.Add(product);// Adcionanamos al contexto el producto
                    await _context.SaveChangesAsync();//Guardamos en la Db
                    return RedirectToAction(nameof(Index));//Redireccionamos al index de productos
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

            model.Categories = _combosHelper.GetComboCategories();//Si el modelo no es valido desplegamos nuevamente el comboCategories
            return View(model);
        }

    }
}
