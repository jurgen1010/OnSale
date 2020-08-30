using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnSale.Common.Entities;
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

        public IEnumerable<SelectListItem> GetComboCities(int departmentId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            Department department = _context.Departments
                .Include(d => d.Cities)
                .FirstOrDefault(d => d.Id == departmentId);
            if (department != null)
            {
                list = department.Cities.Select(t => new SelectListItem
                {
                    Text = t.Name,
                    Value = $"{t.Id}"
                })
                    .OrderBy(t => t.Text)
                    .ToList();
            }

            list.Insert(0, new SelectListItem
            {
                Text = "[Select a city...]",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboCountries()
        {
            List<SelectListItem> list = _context.Countries.Select(t => new SelectListItem
            {
                Text = t.Name,//Valor que mostramos
                Value = $"{t.Id}"//Pero el valor que almacenamos es el Id del pais
            })
                .OrderBy(t => t.Text)
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Select a country...]",//La primera opcion de mi combo sera Select a country...
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboDepartments(int countryId)
        {
            List<SelectListItem> list = new List<SelectListItem>();//Armamos la lista vacia porque nos sabemos si van a ver
            Country country = _context.Countries
                .Include(c => c.Departments)
                .FirstOrDefault(c => c.Id == countryId);//Buscamos el pais con los departamentos
            if (country != null)
            {
                list = country.Departments.Select(t => new SelectListItem//Agreamos los departamentos a mi lista en caso de encontrar un pais
                {
                    Text = t.Name,
                    Value = $"{t.Id}"
                })
                    .OrderBy(t => t.Text)
                    .ToList();
            }

            list.Insert(0, new SelectListItem
            {
                Text = "[Select a department...]",
                Value = "0"
            });

            return list;//devolvemos nuestra lista
        }

    }
}
