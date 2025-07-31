using System.ComponentModel.DataAnnotations;

namespace Fruitables.Models
{
    public class coupon
    {
        [Key]
        public int cId { get; set; }

        public string cCode { get; set; }

        public int discount { get; set; }
    }
}
