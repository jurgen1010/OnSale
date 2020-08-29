using Microsoft.AspNetCore.Mvc.Rendering;
using OnSale.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnSale.Web.Helpers
{
    public class CombosHelper : ICombosHelper
    {
        private readonly DataContext _context;

        //Inyectamos la configuracion de la Db usando el DataContext
        public CombosHelper(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetComboCategories()
        {
            //Iremos a la Db y hacemos una conversion por cada categoria vamos a crear un nuevo SelectListItem
            List<SelectListItem> list = _context.Categories.Select(t => new SelectListItem
            {
                Text = t.Name,//Valor que muestra
                Value = $"{t.Id}"//Valor que almacena $ interpolamos para convertirlo en un String
            })
                .OrderBy(t => t.Text)//Ordemamos por el Texto al combo
                .ToList();

            //Creamos en el primer indice del combo la opcion Select a category...
            list.Insert(0, new SelectListItem
            {
                Text = "[Select a category...]",
                Value = "0"
            });

            return list;
        }

    }
}
