using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicApi.Models
{
    public class User
    {
        public int Id { get; set; }
        [NotMapped]
        public string JWT { get; set; }
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
    }
}