using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OnSale.Common.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} must contain less than {1} characteres")]
        [Required]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)] //Para poder escribir una especie de parrafo en la descripcion se usa la notacion Data.MultilineText
        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]// damos formato al precio de simbolo de modena con C2 quiere decir con 2 decimales

        public decimal Price { get; set; }

        [DisplayName("Is Active")]
        public bool IsActive { get; set; }

        [DisplayName("Is Starred")]//Articulos destacados
        public bool IsStarred { get; set; }

        public Category Category { get; set; }//Indicamos que un producto pertenece a una categoria

        public ICollection<ProductImage> ProductImages { get; set; }//Un productó puede tener muchas imagenes

        [DisplayName("Product Images Number")]
        public int ProductImagesNumber => ProductImages == null ? 0 : ProductImages.Count;

        //TODO: Pending to put the correct paths
        [Display(Name = "Image")]
        public string ImageFullPath => ProductImages == null || ProductImages.Count == 0//Indicamos que si el producto no tiene imagenes, mostramos en pantalla que no tiene imagenes
            ? $"https://localhost:44390/images/noimage.png"
            : ProductImages.FirstOrDefault().ImageFullPath;//sino diremos que de la coleccion ProductImages la imagen principal
    }

}
