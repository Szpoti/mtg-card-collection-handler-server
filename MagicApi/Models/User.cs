using System.ComponentModel.DataAnnotations;

namespace MagicApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string APIKey { get; set; }
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
    }
}