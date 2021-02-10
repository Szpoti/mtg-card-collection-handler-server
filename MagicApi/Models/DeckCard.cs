using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicApi.Models
{
    public class DeckCard
    {
        public int Id { get; set; }
        [Required]
        public string CardId { get; set; }
        [Required]
        public int DeckId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}