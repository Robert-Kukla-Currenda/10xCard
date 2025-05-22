using System.ComponentModel.DataAnnotations;

namespace TenXCards.API.Models
{
    // -----------------------------------------
    // User DTOs
    // -----------------------------------------
    
    /// <summary>
    /// Represents the user data to be sent in responses.
    /// Derived from the User entity.
    /// </summary>
    public class UserDto
    {
        public int Id { get; set; }
        
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = default!;
        
        [StringLength(100)]
        public string FirstName { get; set; } = default!;
        
        [StringLength(100)]
        public string LastName { get; set; } = default!;
        
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Represents the login response, including the JWT token and user information.
    /// </summary>
    public class LoginResultDto
    {
        public string Token { get; set; } = default!;
        public UserDto User { get; set; } = default!;
    }

    // -----------------------------------------
    // Card DTOs
    // -----------------------------------------

    /// <summary>
    /// Represents a flashcard.
    /// Derived from the Card entity.
    /// </summary>
    public class CardDto
    {
        public int Id { get; set; }        

        [MinLength(1)]
        [MaxLength(1000)]
        public string Front { get; set; } = default!;

        [MinLength(1)]
        [MaxLength(5000)]
        public string Back { get; set; } = default!;

        [StringLength(10)]
        public string GeneratedBy { get; set; } = default!;

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public UserDto? User { get; set; }
        public OriginalContentDto? OriginalContent { get; set; }        
    }

    // -----------------------------------------
    // Original Content DTO
    // -----------------------------------------

    /// <summary>
    /// Represents the original content associated with a user.
    /// Derived from the OriginalContent entity.
    /// </summary>
    public class OriginalContentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [StringLength(10000, MinimumLength = 1000)]
        public string Content { get; set; } = default!;

        public DateTime CreatedAt { get; set; }
        
        // Navigation property
        public ICollection<CardDto> Cards { get; set; } = new List<CardDto>();
    }

    // -----------------------------------------
    // Error Log DTO
    // -----------------------------------------
    
    /// <summary>
    /// Represents an error log associated with a flashcard.
    /// Derived from the ErrorLog entity.
    /// </summary>
    public class ErrorLogDto
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        
        public string ErrorDetails { get; set; } = default!;
        
        public DateTime LoggedAt { get; set; }
    }
}