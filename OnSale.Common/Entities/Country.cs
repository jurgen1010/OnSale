using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;

namespace OnSale.Common.Entities
{
    public class Country
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage ="The field {0} must contain less than {1} characteres") ]
        [Required]//con dicha notacion puedo indicarle al framework annotacion que este campo es obligatorio
        public string Name { get; set; }

    }
}
