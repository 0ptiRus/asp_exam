using exam_frontend.Entities;
using exam_frontend.Models;

namespace exam_frontend.Models;

public class PostModel
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    
    public List<CommentModel> Comments { get; set; } = new();
}