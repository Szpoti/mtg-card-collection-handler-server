using System.ComponentModel.DataAnnotations;

namespace MagicApi.Models
{
    public class Deck
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        public Format Format { get; set; }

    }
}