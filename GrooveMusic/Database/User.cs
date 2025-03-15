using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace GrooveMusic.Database
{
    public class User : Document
    {
        public string userId { get; set; }
        [Required]
        public string userName { get; set; }
        public List<string> emailId { get; set; } = new List<string>();
        public List<MobileNumber> mobileNumber = new List<MobileNumber>();
        public string pin {  get; set; }
        public string password { get; set; }
        public string? recoveryemail { get; set; }

    }

    public class MobileNumber
    {
        public string mobileCode { get; set; }
        public string mobileNumber {  get; set; }
    
    }
}
