using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Fruitables.Models
{
    public class wishlist
    {
        [Key]
        public int wlistId { get; set; }

        public int prodId { get; set; }

        // Navigation property
        [ForeignKey("prodId")]
        public product product { get; set; }

        public int userId { get; set; }
        [ForeignKey("userId")]
        public user user { get; set; }
    }
}
