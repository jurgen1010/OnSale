using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OnSale.Common.Entities
{
    public class OrderDetail//No depende de user por lo cual podemos empaquetarla en commons
    {
        public int Id { get; set; }

        public Product Product { get; set; }

        public float Quantity { get; set; }

        public decimal Price { get; set; }// En su momento el precio puede cambiar es por eso que no solo debe depender de la tabla producto sino que en la orden del pedido tambien
        //La 2da forma normal no siempre se cumplira al pie de la letra por cuestiones de logica como pasa con este campo
        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }

        public decimal Value => (decimal)Quantity * Price;
    }

}
