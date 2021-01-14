using System.ComponentModel.DataAnnotations;

namespace MagicApi.Models
{
    public class Format
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int? minCardNumber { get; set; }

        public int? maxCardNumber { get; set; }
    }
}