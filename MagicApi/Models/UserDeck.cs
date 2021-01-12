using System.ComponentModel.DataAnnotations;

namespace MagicApi.Models
{
    public class UserDeck
    {
        public int Id { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        public Format Format { get; set; }
    }
}