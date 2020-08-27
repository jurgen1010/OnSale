using System;
using System.ComponentModel.DataAnnotations;

namespace OnSale.Common.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} must contain less than {1} characteres")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Image")]//Usamos la notacion Display para que lo presente al usuario como Image
        //Guid 
        public Guid ImageId { get; set; }//Vamos a generar un codigo unico a cada imagen, con un Guid es una coleccion alfanumerica que no se repite

        //TODO: Pending to put the correct paths
        [Display(Name = "Image")]
        public string ImageFullPath => ImageId == Guid.Empty //Creamos una propiedad de lectura donde van a estar todas las imagenes 
            ? $"https://localhost:44312/images/noimage.png" //Si no hay imagen vamos a guardar una imagen llamada noimage.png
            : $"https://onsale.blob.core.windows.net/categories/{ImageId}"; // las imagenes quedaran en un blob storage y le enviamos el codigo de la imagen{ImageId}
    }

}
