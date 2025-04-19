using System.ComponentModel.DataAnnotations;

namespace TenXCards.API.Models
{
    // -----------------------------------------
    // User Commands
    // -----------------------------------------
    
    /// <summary>
    /// Command model used when registering a new user.
    /// </summary>
    public class RegisterUserCommand
    {
        [Required(ErrorMessage = "Adres email jest wymagany.")]
        [EmailAddress(ErrorMessage = "Podany tekst nie jest prawidłowym adresem email.")]
        [StringLength(255, ErrorMessage = "Adres email nie może być dłuższy niż {1} znaków.")]
        public string Email { get; set; } = default!;
        
        [Required(ErrorMessage = "Imię jest wymagane.")]
        [StringLength(100, ErrorMessage = "Imię nie może być dłuższe niż {1} znaków.")]
        public string FirstName { get; set; } = default!;
        
        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        [StringLength(100, ErrorMessage = "Nazwisko nie może być dłuższe niż {1} znaków.")]
        public string LastName { get; set; } = default!;
        
        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [StringLength(100, MinimumLength = 6, 
            ErrorMessage = "Hasło musi zawierać od {2} do {1} znaków.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", 
            ErrorMessage = "Hasło musi zawierać co najmniej jedną małą literę, jedną dużą literę i jedną cyfrę.")]
        public string Password { get; set; } = default!;
    }

    /// <summary>
    /// Command model used during user authentication.
    /// </summary>
    public class LoginUserCommand
    {
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = default!;
        
        [Required]
        public string Password { get; set; } = default!;
    }

    // -----------------------------------------
    // Card Commands
    // -----------------------------------------

    /// <summary>
    /// Command model for creating a manual flashcard.
    /// </summary>
    public class SaveCardCommand
    {
        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string Front { get; set; } = default!;

        [Required]
        [StringLength(5000, MinimumLength = 1)]
        public string Back { get; set; } = default!;

        // Possible values: "AI" or "human"
        [Required]
        [StringLength(10)]
        public required string GeneratedBy { get; set; }

        // Field for original content
        [StringLength(10000, MinimumLength = 1000)]
        public string OriginalContent { get; set; } = default!;
    }

    /// <summary>
    /// Command model for generating a flashcard via AI.
    /// </summary>
    public class GenerateCardCommand
    {
        [Required]
        [StringLength(10000, MinimumLength = 1000)]
        public string OriginalContent { get; set; } = default!;
    }

    /// <summary>
    /// Command model for updating an existing flashcard.
    /// Only allows updating Front and Back.
    /// </summary>
    public class UpdateCardCommand
    {
        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string Front { get; set; } = default!;
        
        [Required]
        [StringLength(5000, MinimumLength = 1)]
        public string Back { get; set; } = default!;
    }
}