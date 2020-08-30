using OnSale.Common.Entities;
using OnSale.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnSale.Web.Data.Entities
{
    public class Order// va dentro de mi capa Web porque depende de usuario como se puede ver en la linea 16
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public User User { get; set; }

        public OrderStatus OrderStatus { get; set; }

        [Display(Name = "Date Sent")]
        public DateTime? DateSent { get; set; }// es un campo nullo hasta que no se haga el envio

        [Display(Name = "Date Confirmed")]// es un campo nullo hasta que no se confirme
        public DateTime? DateConfirmed { get; set; }

        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }//Comentario a nivel de la orden

        public ICollection<OrderDetail> OrderDetails { get; set; }

        public int Lines => OrderDetails == null ? 0 : OrderDetails.Count;

        public float Quantity => OrderDetails == null ? 0 : OrderDetails.Sum(od => od.Quantity);

        public decimal Value => OrderDetails == null ? 0 : OrderDetails.Sum(od => od.Value);

    }

}
