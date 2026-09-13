using Entities;
using RepositoryContracts;
namespace CLI.UI.ManagePosts;

public class ListPostsView
{
    private readonly IPostRepository postRepository;

    public ListPostsView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public Task ListPostsAsync()
    {
        Console.WriteLine("\nAll Posts");
        IQueryable<Post> posts = postRepository.GetMany();
        foreach (Post post in posts)
        {
            Console.WriteLine($"- ID: {post.Id}   Title: {post.Title}");
        }
        return Task.CompletedTask;
    }
}