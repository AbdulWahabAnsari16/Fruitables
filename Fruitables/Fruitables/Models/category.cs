using System.ComponentModel.DataAnnotations;

namespace Fruitables.Models
{
    public class category
    {
        [Key]
        public int catId { get; set; }

        public string catName { get; set; }

        public ICollection<product> product { get; set; }
    }
}
