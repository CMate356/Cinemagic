using System.ComponentModel.DataAnnotations;

namespace Cinemagic.Models
{
    public class AddressInputViewModel
    {
        [Required(ErrorMessage = "Strada este obligatorie.")]
        [StringLength(200)]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "Orașul este obligatoriu.")]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefonul este obligatoriu.")]
        [StringLength(20)]
        [RegularExpression(@"^[0-9+\-\s().]*$", ErrorMessage = "Telefon invalid.")]
        public string Phone { get; set; } = string.Empty;

        public bool MakeDefault { get; set; }
    }
}