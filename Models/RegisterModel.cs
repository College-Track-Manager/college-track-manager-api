using System.ComponentModel.DataAnnotations;

namespace CollegeTrackAPI.Models
{
    public class RegisterModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "The national ID must be exactly 14 digits.")]
        public string NationalId { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }

        // Simplified password validation (no complex rules)
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password should be at least 6 characters long.")]
        public string Password { get; set; }

        // Confirm password with no complexity check
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
