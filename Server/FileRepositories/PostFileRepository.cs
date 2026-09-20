using System.IO;
using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;


public class PostFileRepository : IPostRepository
{
    private readonly string filePath = "post.json";//Here we define the file path. We create a file per entity.
   
    
    public PostFileRepository()
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

    private async Task<List<Post>> LoadPostsAsync() 
        //Helper methods (reads the file, deserialize, returns the list of posts)
    {
        string json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Post>>(json) ?? new List<Post>();
    }
    
    private async Task SavePostsAsync(List<Post> posts)
        //Serializes the provided list and writes it back to file
    {
        string json = JsonSerializer.Serialize(posts, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }
    
    
    
    public async Task<Post> AddAsync(Post post) //his is the method header, asynchronous, returning a Task containing the “finalized” post, i.e., it now has an ID
    {
        //use helper to load posts, removing code duplication
        List<Post> posts = await LoadPostsAsync(); 
        
        int maxId = posts.Count > 0 ? posts.Max(u => u.Id) : 0; 
        post.Id = maxId + 1;
        
        posts.Add(post); 
        
        // We use the helper to save the changes.
        await SavePostsAsync(posts); 
        
        return post; 
        
        
        /*string postsAsJson = await File.ReadAllTextAsync(filePath); // We read all the content from the file; this is, of course, in JSON format.
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postsAsJson) ?? new List<Post>(); //The JSON is deserialized into a list of posts.
        int maxId = posts.Count > 0 ? posts.Max(u => u.Id) : 0; // We calculate the next ID to use.
        post.Id = maxId + 1;//Set the ID.
        posts.Add(post); //Add the post to the list.
        postsAsJson = JsonSerializer.Serialize(posts, new JsonSerializerOptions { WriteIndented = true }); //Serialize the list to JSON.
        await File.WriteAllTextAsync(filePath, postsAsJson); // Write the JSON back to the file.
        
        return post; //Return the finalized post now that it has an ID.*/
    }
    
   
    
    
    
    public async Task UpdateAsync(Post post)
    {
        List<Post> posts = await LoadPostsAsync();

        Post? existingPost = posts.SingleOrDefault(u => u.Id == post.Id);
        if (existingPost is null)
        {
            throw new InvalidOperationException($"Post with ID '{post.Id}' not found");
        }

        posts.Remove(existingPost);
        posts.Add(post);

        await SavePostsAsync(posts);
    }

    public async Task DeleteAsync(int id)
    {
        List<Post> posts = await LoadPostsAsync();
        
        var postToRemove = posts.SingleOrDefault(u => u.Id == id);
        if (postToRemove is null)
        {
            throw new InvalidOperationException($"Post with ID '{id}' not found");
        }

        posts.Remove(postToRemove);
        await SavePostsAsync(posts);
    }

    public async Task<Post> GetSingleAsync(int id)
    {
        List<Post> posts = await LoadPostsAsync();
        
        Post? post = posts.SingleOrDefault(u => u.Id == id);
        if (post is null)
        {
            throw new InvalidOperationException($"Post with ID '{id}' not found");
        }

       //return Task.FromResult(post); the methoid is async,so plain object it returnesd
       return post;
    }

    public IQueryable<Post> GetMany()
    {
        /*string postsAsJson = File.ReadAllTextAsync(filePath).Result; //ReadAllTextAsync() returns a Task containing a string calling result instead of awaiting
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postsAsJson)!;
        return posts.AsQueryable();*/
        
        List<Post> posts = LoadPostsAsync().Result; //Result instead of awaiting will extract the string. This is generally not advisable because we lose the async behavior and optimization, but in this case, the method header is not async as defined in the interface, so we have to “cheat.”
        return posts.AsQueryable();
        //previous version repeats the load logic and ends with !, which would crash if the file ever contained null.
        //Helper (LoadPostsAsync()) handles that case.Result would safer and shorter.
    }
}
