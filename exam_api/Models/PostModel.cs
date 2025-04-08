using exam_api.Entities;

namespace exam_api.Models;

public class PostModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    
    public List<CommentModel> Comments { get; set; } = new();
}