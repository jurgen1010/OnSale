using Microsoft.AspNetCore.Http;
using OnSale.Common.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnSale.Web.Models
{
    public class CategoryViewModel : Category//En el MVC los ViewModel son Models
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }// IFormeFile nos permite capturar la imagen en memoria
    }

}
