using System.ComponentModel.DataAnnotations;

namespace Cinemagic.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Numele complet este obligatoriu.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Numele trebuie să aibă între 2 și 100 de caractere.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email-ul este obligatoriu.")]
        [EmailAddress(ErrorMessage = "Adresă de email invalidă.")]
        [StringLength(150, ErrorMessage = "Email-ul poate avea cel mult 150 de caractere.")]
        public string Email { get; set; } = string.Empty;

        // BCrypt foloseste maximum 72 de caractere din parola
        [Required(ErrorMessage = "Parola este obligatorie.")]
        [DataType(DataType.Password)]
        [StringLength(72, MinimumLength = 8, ErrorMessage = "Parola trebuie să aibă între 8 și 72 de caractere.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$", ErrorMessage = "Parola trebuie să conțină cel puțin o literă și o cifră.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmarea parolei este obligatorie.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Parolele nu coincid.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}