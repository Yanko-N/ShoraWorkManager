using System.ComponentModel.DataAnnotations;

namespace Persistence.Models
{
    public class AuthorizationToken
    {
        [Key]
        public int Id { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime ExpirationDate { get; set; } = DateTime.Now.AddHours(1);
        
        [Required]
        public bool IsUsed { get; set; } = false;
    }
}
