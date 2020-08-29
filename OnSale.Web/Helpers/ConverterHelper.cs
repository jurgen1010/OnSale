using Microsoft.IdentityModel.Tokens;
using OnSale.Common.Entities;
using OnSale.Web.Data;
using OnSale.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnSale.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;
        

        //Inyectamos todas nuestras implementaciones nuevamente
        public ConverterHelper(DataContext context, ICombosHelper combosHelper)
        {
            _context = context;
            _combosHelper = combosHelper;
        }

        public DataContext Context { get; }
        public ICombosHelper CombosHelper1 { get; }
        public CombosHelper CombosHelper { get; }

        public Category ToCategory(CategoryViewModel model, Guid imageId, bool isNew)
        {
            return new Category
            {
                Id = isNew ? 0 : model.Id,//Sí el atributo es nuevo, enviamos 0 para que lo inserte en la Db sino enviamos model:Id
                ImageId = imageId,
                Name = model.Name
            };
        }

        public CategoryViewModel ToCategoryViewModel(Category category)
        {
            return new CategoryViewModel
            {
                Id = category.Id, // Enviamos una nueva CategoryViewModel con el Id de la category,imageId y Name
                ImageId = category.ImageId,
                Name = category.Name
            };
        }

        public async Task<Product> ToProductAsync(ProductViewModel model, bool isNew)
        {
            return new Product//Retornamos el nuevo producto
            {
                Category = await _context.Categories.FindAsync(model.CategoryId),//FindAsync busca por clave primaria, donde bucamos la categoria por Id
                Description = model.Description,
                Id = isNew ? 0 : model.Id,//Si es nuevo es porque su Id es 0 si es una edicion tomamos el Id de el model
                IsActive = model.IsActive,
                IsStarred = model.IsStarred,
                Name = model.Name,
                Price = model.Price,
                ProductImages = model.ProductImages
            };
        }

        public ProductViewModel ToProductViewModel(Product product)
        {
            return new ProductViewModel//Retornamos un nuevo ProductViewModel con todos sus atributos
            {
                Categories = _combosHelper.GetComboCategories(),
                Category = product.Category,
                CategoryId = product.Category.Id,
                Description = product.Description,
                Id = product.Id,
                IsActive = product.IsActive,
                IsStarred = product.IsStarred,
                Name = product.Name,
                Price = product.Price,
                ProductImages = product.ProductImages
            };

        }
    }
}
