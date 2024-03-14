using System.ComponentModel.DataAnnotations;

namespace Backend.Data.DTOs.Request
{
  public class NewPasswordDTO
  {
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public string OldPassword { get; set; } = string.Empty;
    [Required]
    public string NewPassword { get; set; } = string.Empty;
  }
}
