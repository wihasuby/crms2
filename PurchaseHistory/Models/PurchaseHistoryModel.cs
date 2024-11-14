using System;
using System.ComponentModel.DataAnnotations;

namespace crms2.PurchaseHistory.Models
{
    public class PurchaseHistoryModel
    {
        [Required]
        public int CustomerId { get; set; } // Foreign key reference to the Customers table

        public string CustomerEmail { get; set; } // Used for lookup, not stored in the database

        [Required]
        [StringLength(100, ErrorMessage = "Purchasable item name cannot exceed 100 characters.")]
        public string Purchasable { get; set; } // The item or service purchased

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; } // Price of the item

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } // Quantity of the item purchased

        public decimal Total => Price * Quantity; // Calculated total for the purchase

        [Required]
        public DateTime PurchaseDate { get; set; } = DateTime.Now; // Date of the purchase
    }
}
