using System.ComponentModel.DataAnnotations;

namespace PMQLSV.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
