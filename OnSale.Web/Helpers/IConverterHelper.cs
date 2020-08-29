using OnSale.Common.Entities;
using OnSale.Web.Models;
using System;
using System.Threading.Tasks;

namespace OnSale.Web.Helpers
{
    public interface IConverterHelper
    {
        //Nos ayudara a convertir un controlador tipo CategoryViewModel a una Category
        Category ToCategory(CategoryViewModel model, Guid imageId, bool isNew);

        //En este caso una Category a un CategoryViewModel
        CategoryViewModel ToCategoryViewModel(Category category);

        //Este metodo es asincrono porque cuando el user selecciona en la vista me devuelve la categoria (id) para luego buscarlo con dicho (id) y se recomienda que se haga asincrona
        Task<Product> ToProductAsync(ProductViewModel model, bool isNew);

        ProductViewModel ToProductViewModel(Product product);


    }
}
