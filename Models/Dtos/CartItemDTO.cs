using System;
using System.ComponentModel.DataAnnotations;

namespace Perkebunan.Models.Dtos
{
    public class CartItemDTO
    {
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
