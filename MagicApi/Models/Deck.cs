using System.ComponentModel.DataAnnotations;

namespace MagicApi.Models
{
    public class Deck
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int FormatId { get; set; }

    }
}