using Microsoft.AspNetCore.Identity;
namespace exam_api.Entities;
public class ApplicationUser : IdentityUser
{
    public IEnumerable<Gallery> Galleries { get; set; }
    public IEnumerable<Post> Posts { get; set; }
    public IEnumerable<Follow> Followers { get; set; }
    public IEnumerable<Follow> Followed { get; set; }
    public UploadedFile Pfp { get; set; }
}