using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyStore.Model
{
    [Owned]
    public class Price
    {
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; } = null!; 

        public Price() { } 
        public Price(decimal amount, string currency)
        {
            if (amount < 0) throw new ArgumentException("Price must be non-negative.");
            Amount = amount;
            Currency = currency;
        }
    }
}
