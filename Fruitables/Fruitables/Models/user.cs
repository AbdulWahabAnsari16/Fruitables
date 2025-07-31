using System.ComponentModel.DataAnnotations;

namespace Fruitables.Models
{
    public class user
    {
        [Key]
        public int userId { get; set; }
        public string userName { get; set; }
        public string userEmail { get; set; }
        public string userPass { get; set; }
    }
}
