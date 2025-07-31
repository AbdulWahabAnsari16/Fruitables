using System.ComponentModel.DataAnnotations;

namespace Fruitables.Models
{
    public class verificationCode
    {
        [Key]
        public int Id { get; set; }
        public string vCode { get; set; }
    }
}
