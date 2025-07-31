using System.ComponentModel.DataAnnotations;

namespace Fruitables.Models
{
    public class contact
    {
        [Key]
        public int contactId { get; set; }

        public string contactName { get; set; }

        public string contactEmail { get; set; }

        public string contactMsg { get; set; }
    }
}
