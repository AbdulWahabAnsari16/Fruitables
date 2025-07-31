using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fruitables.Models
{
    public class product
    {
        [Key]
        public int prodId { get; set; }

        public string prodName { get; set; }

        public int prodPrice { get; set; }

        public string prodDesc { get; set; }

        public int prodQty { get; set; }

        [NotMapped]
        public IFormFile ProductImage { get; set; }

        public string PImage { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public category category { get; set; }
        public ICollection<addtocart> addtocarts { get; set; }
    }
}
