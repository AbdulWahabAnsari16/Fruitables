using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fruitables.Models
{
    public class addtocart
    {
        [Key]
        public int atcId { get; set; }

        public int prodId { get; set; }

        // Navigation property
        [ForeignKey("prodId")]
        public product product { get; set; }

        public int userId { get; set; }
        [ForeignKey("userId")]
        public user user { get; set; }

        // Add this property
        public int Quantity { get; set; } = 1; // Default to 1
    }
}
