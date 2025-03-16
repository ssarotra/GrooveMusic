using System.ComponentModel.DataAnnotations;

namespace GrooveMusic.Models
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
