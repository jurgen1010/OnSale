using OnSale.Common.Entities;
using OnSale.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnSale.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
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

    }
}
