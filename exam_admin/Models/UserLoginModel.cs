using System.ComponentModel.DataAnnotations;

namespace exam_admin.Models;

public class UserLoginModel
{
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}