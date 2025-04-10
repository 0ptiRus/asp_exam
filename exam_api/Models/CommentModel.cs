namespace exam_api.Models;

public class CommentModel
{
    public int Id { get; set; }
    
    public int PostId { get; set; }
    
    public string UserId { get; set; }
    public string Text { get; set; }
    public string UserName { get; set; }
    public string ProfilePictureUrl { get; set; }
}