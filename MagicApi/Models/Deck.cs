using System.ComponentModel.DataAnnotations;

namespace MagicApi.Models
{
    public class Deck
    {
        public int Id { get; set; }
        [Required]
        public string CardId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}