using System;
using System.Collections.Generic;
using System.Text;

namespace New.Common.Entity
{
    class Count
    {
        public int Id { get; set; }

        [MaxLength(50)]
        [Required]//con dicha notacion puedo indicarle al framework annotacion que este campo es obligatorio
        public string Name { get; set; }
    }
}
