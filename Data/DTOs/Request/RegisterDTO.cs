using System.ComponentModel.DataAnnotations;

namespace Backend.Data.DTOs.Request
{
  public class RegisterDTO
  {
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    [Required, EmailAddress]
    public string Email { get; set; }
  }
}
