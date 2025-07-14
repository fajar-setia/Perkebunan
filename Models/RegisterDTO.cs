using System.ComponentModel.DataAnnotations;

namespace Perkebunan.Models
{
    public class RegisterDTO
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
        public RegisterDTO() { }
        public RegisterDTO(string username, string fullName, string email, string password, string confirmPassword)
        {
            Username = username;
            FullName = fullName;
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
        }

    }
}
