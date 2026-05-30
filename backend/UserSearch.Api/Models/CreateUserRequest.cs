using System.ComponentModel.DataAnnotations;

namespace UserSearch.Api.Models;

public class CreateUserRequest
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string JobTitle { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^07\d{3}\s?\d{6}$", ErrorMessage = "Phone must be a valid UK mobile number (e.g. 07789 543768)")]
    public string Phone { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;
}
