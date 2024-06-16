using System.ComponentModel.DataAnnotations;

namespace Identity.Models.ViewModel
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
    }
}
