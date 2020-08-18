using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnSale.Common.Entities
{
    public class Department
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} must contain less than {1} characteres")]
        [Required]
        public string Name { get; set; }

        public ICollection<City> Cities { get; set; }//Estamos indicando que un departamento tiene una coleccion de ciudades

        //Creamos una propiedad de lectura, indicamos que sí la coleccion de 
        //ciudades es nulla la respuesta sera igual a 0 de lo contrario devuelvame el conteo de las ciudades
        [DisplayName("Cities Number")]
        public int CitiesNumber => Cities == null ? 0 : Cities.Count;

        [JsonIgnore]
        [NotMapped]
        public int IdCountry { get; set; }


    }
}
