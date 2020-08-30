using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnSale.Common.Entities
{
    public class City
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} must contain less than {1} characteres")]
        [Required]
        public string Name { get; set; }

        [JsonIgnore]//Al generar la respuesta del servicio no ira este campo
        [NotMapped]//con dicha notacion indico que no quiero que se Mapee este campo en la DB
        public int IdDepartment { get; set; }
        [JsonIgnore]//Para no generar una redundancia ciclica en el API
        public Department Department { get; set; }//Una ciudad pertenece a un departamento


    }
}
