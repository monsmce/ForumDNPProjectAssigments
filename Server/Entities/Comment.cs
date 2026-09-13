namespace Entities;

public class Comment
{
    public int Id { get; set; }
    public string Body { get; set; }
    
    public int PostId { get; } // no sense to modify after creation
    public int UserId { get; } //
    
    public Comment(string body, int postId, int userId)
    {
        Body = body;
        PostId = postId;
        UserId = userId;
    }
    //instantiate a Comment using object initializer. constructor instantiates an object with less code.
}