using System.ComponentModel.DataAnnotations;

namespace Cinemagic.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Numele complet este obligatoriu.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email-ul este obligatoriu.")]
        [EmailAddress(ErrorMessage = "Adresă de email invalidă.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parola este obligatorie.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Parola trebuie să aibă cel puțin 6 caractere.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmarea parolei este obligatorie.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Parolele nu coincid.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}