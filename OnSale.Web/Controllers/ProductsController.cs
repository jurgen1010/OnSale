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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product product = await _context.Products
                .Include(p => p.Category)//Buscamos la categoria a la cual pertenece el producto 
                .Include(p => p.ProductImages)//Adicional traemos las imagenes del producto
                .FirstOrDefaultAsync(p => p.Id == id);// y tambien mostramos la imagen principal
            if (product == null)
            {
                return NotFound();
            }

            ProductViewModel model = _converterHelper.ToProductViewModel(product);//Convertimos el model product recuperado a un viewModel para retornarlo a la vista
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            if (ModelState.IsValid)//La vista nos regresa al POST para que podamos actualizar en la base de datos el producto, validamos que el modelo sea valido
            {
                try
                {
                    Product product = await _converterHelper.ToProductAsync(model, false);//Convertimos de nuevo el model a un Product para poderlo salvar en la Db, con parametro false porque no es nuevo

                    if (model.ImageFile != null)//Validamos si adicionaron una imagen
                    {
                        Guid imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "products");//Entonces creamos un imageId para cargarlo al blob
                        if (product.ProductImages == null)
                        {
                            product.ProductImages = new List<ProductImage>();
                        }

                        product.ProductImages.Add(new ProductImage { ImageId = imageId });
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();//Actualizamos el producto en la Db
                    return RedirectToAction(nameof(Index));

                }
                catch (DbUpdateException dbUpdateException)// Manejamos las posibles excepciones
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

            model.Categories = _combosHelper.GetComboCategories();//Si no es valido el modelo aramamos nuevamente nuevamente el comboCategories
            return View(model);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product product = await _context.Products
                .Include(p => p.ProductImages)//Incluimos las imagenes para que tambien haga borrado de ellas 
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            try
            {
                _context.Products.Remove(product);//lo hacemos en un try por si el borrado de este producto tiene integridad referencial no se reviente el proyecto
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product product = await _context.Products
                .Include(c => c.Category)//Incluimos la categoria
                .Include(c => c.ProductImages)//Incluimos las imagenes del producto y finalmente retornamos a la vista para presentarlo en pantalla
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        public async Task<IActionResult> AddImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            AddProductImageViewModel model = new AddProductImageViewModel { ProductId = product.Id };//En esta parte le indicamos a cual model le vamos a cargar la imagen
            return View(model); //Retornamos a la view el producto al cual se va adicionar las imagenes
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImage(AddProductImageViewModel model)
        {
            if (ModelState.IsValid)
            {
                Product product = await _context.Products
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.Id == model.ProductId);
                if (product == null)
                {
                    return NotFound();
                }

                try
                {
                    Guid imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "products");
                    if (product.ProductImages == null)//Si la coleccion de imagenes es nulla es poruqe vamos agregar nuevas imagenes
                    {
                        product.ProductImages = new List<ProductImage>();//Entonces creamos una nueva coleccion de imagenes 
                    }

                    product.ProductImages.Add(new ProductImage { ImageId = imageId });// A la coleccion de imagenes agreguele una nueva imagen
                    _context.Update(product);//Actualizamos el contexto de la Db
                    await _context.SaveChangesAsync();//salvamos en la Db los cambios
                    return RedirectToAction($"{nameof(Details)}/{product.Id}");//Retornamos a al detalle del producto actualizado

                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            return View(model);
        }
        public async Task<IActionResult> DeleteImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductImage productImage = await _context.ProductImages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productImage == null)
            {
                return NotFound();
            }

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.ProductImages.FirstOrDefault(pi => pi.Id == productImage.Id) != null);
            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();
            return RedirectToAction($"{nameof(Details)}/{product.Id}");
        }

    }
}
