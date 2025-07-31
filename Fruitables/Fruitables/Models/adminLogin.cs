using System.ComponentModel.DataAnnotations;

namespace Fruitables.Models
{
    public class adminLogin
    {
        [Key]
        public int adminId { get; set; }
        public string adminName { get; set; }
        public string adminEmail { get; set; }
        public string adminPass { get; set; }

    }
}
