using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace OnSale.Web.Helpers
{
    public interface ICombosHelper
    {
        IEnumerable<SelectListItem> GetComboCategories();
        IEnumerable<SelectListItem> GetComboCountries();
        IEnumerable<SelectListItem> GetComboDepartments(int countryId);//Me interesa solo el combo de departamentos del Pais seleccionado
        IEnumerable<SelectListItem> GetComboCities(int departmentId);

    }
}
