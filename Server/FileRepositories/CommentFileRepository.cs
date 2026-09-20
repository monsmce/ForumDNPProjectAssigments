using System.IO;
using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;


public class CommentFileRepository : ICommentRepository
{
    private readonly string filePath = "comment.json";//Here we define the file path. We create a file per entity.
   
    
    public CommentFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }
    // The constructor ensures there actually is a file.
    // If none exists (e.g., the first time the program is run),
    // a new file is created with the content of an empty list,
    // i.e., no entities. The "[]" is an empty collection in JSON.

    private async Task<List<Comment>> LoadCommentsAsync() 
        //Helper methods (reads the file, deserialize, returns the list of comments)
    {
        string json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Comment>>(json) ?? new List<Comment>();
    }
    
    private async Task SaveCommentsAsync(List<Comment> comments)
        //Serializes the provided list and writes it back to file
    {
        string json = JsonSerializer.Serialize(comments, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }
    
    
    
    public async Task<Comment> AddAsync(Comment comment) //his is the method header, asynchronous, returning a Task containing the “finalized” comment, i.e., it now has an ID
    {
        //use helper to load comments, removing code duplication
        List<Comment> comments = await LoadCommentsAsync(); 
        
        int maxId = comments.Count > 0 ? comments.Max(u => u.Id) : 0; 
        comment.Id = maxId + 1;
        
        comments.Add(comment); 
        
        // We use the helper to save the changes.
        await SaveCommentsAsync(comments); 
        
        return comment; 
        
        
        /*string commentsAsJson = await File.ReadAllTextAsync(filePath); // We read all the content from the file; this is, of course, in JSON format.
        List<Comment> comments = JsonSerializer.Deserialize<List<Comment>>(commentsAsJson) ?? new List<Comment>(); //The JSON is deserialized into a list of comments.
        int maxId = comments.Count > 0 ? comments.Max(u => u.Id) : 0; // We calculate the next ID to use.
        comment.Id = maxId + 1;//Set the ID.
        comments.Add(comment); //Add the comment to the list.
        commentsAsJson = JsonSerializer.Serialize(comments, new JsonSerializerOptions { WriteIndented = true }); //Serialize the list to JSON.
        await File.WriteAllTextAsync(filePath, commentsAsJson); // Write the JSON back to the file.
        
        return comment; //Return the finalized comment now that it has an ID.*/
    }
    
   
    
    
    
    public async Task UpdateAsync(Comment comment)
    {
        List<Comment> comments = await LoadCommentsAsync();

        Comment? existingComment = comments.SingleOrDefault(u => u.Id == comment.Id);
        if (existingComment is null)
        {
            throw new InvalidOperationException($"Comment with ID '{comment.Id}' not found");
        }

        comments.Remove(existingComment);
        comments.Add(comment);

        await SaveCommentsAsync(comments);
    }

    public async Task DeleteAsync(int id)
    {
        List<Comment> comments = await LoadCommentsAsync();
        
        var commentToRemove = comments.SingleOrDefault(u => u.Id == id);
        if (commentToRemove is null)
        {
            throw new InvalidOperationException($"Comment with ID '{id}' not found");
        }

        comments.Remove(commentToRemove);
        await SaveCommentsAsync(comments);
    }

    public async Task<Comment> GetSingleAsync(int id)
    {
        List<Comment> comments = await LoadCommentsAsync();
        
        Comment? comment = comments.SingleOrDefault(u => u.Id == id);
        if (comment is null)
        {
            throw new InvalidOperationException($"Comment with ID '{id}' not found");
        }

       //return Task.FromResult(comment); the methoid is async,so plain object it returnesd
       return comment;
    }

    public IQueryable<Comment> GetMany()
    {
        /*string commentsAsJson = File.ReadAllTextAsync(filePath).Result; //ReadAllTextAsync() returns a Task containing a string calling result instead of awaiting
        List<Comment> comments = JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
        return comments.AsQueryable();*/
        
        List<Comment> comments = LoadCommentsAsync().Result; //Result instead of awaiting will extract the string. This is generally not advisable because we lose the async behavior and optimization, but in this case, the method header is not async as defined in the interface, so we have to “cheat.”
        return comments.AsQueryable();
        //previous version repeats the load logic and ends with !, which would crash if the file ever contained null.
        //Helper (LoadCommentsAsync()) handles that case.Result would safer and shorter.
    }
}
