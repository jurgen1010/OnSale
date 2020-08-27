using OnSale.Common.Entities;
using OnSale.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnSale.Web.Helpers
{
    public interface IConverterHelper
    {
        //Nos ayudara a convertir un controlador tipo CategoryViewModel a una Category
        Category ToCategory(CategoryViewModel model, Guid imageId, bool isNew);

        //En este caso una Category a un CategoryViewModel
        CategoryViewModel ToCategoryViewModel(Category category);

    }
}
