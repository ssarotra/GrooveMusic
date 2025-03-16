using GrooveMusic.Database;
using System.ComponentModel.DataAnnotations;
using static GrooveMusic.Database.User;

public class RegisterRequest
{
    [Required]
    public string userName { get; set; }

    [Required]
    public string password { get; set; }

    [Required]
    public List<string> emailIds { get; set; } = new List<string>();

    [Required]
    public string pin { get; set; }

    public List<MobileNumber> mobileNumbers { get; set; } = new List<MobileNumber>();

    public string? recoveryemail { get; set; }

    // Maximum allowed screens (default: 1)
    public int MaxScreens { get; set; } = 1;
}
