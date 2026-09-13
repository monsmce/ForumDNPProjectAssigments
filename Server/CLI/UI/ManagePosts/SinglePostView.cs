using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class SinglePostView
{
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;

    public SinglePostView(IPostRepository postRepository, ICommentRepository commentRepository, IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
    }

    public async Task ShowPostAsync()
    {
        int postId;
        while (true)
        {
            Console.Write("Post ID: ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out postId))
            { 
                try
                {
                    Post post = await postRepository.GetSingleAsync(postId);

                    if (post != null)
                    {
                        Console.WriteLine($"\n--- {post.Title} ---");
                        Console.WriteLine(post.Body);
                        break;
                    }
                    Console.WriteLine($"Error: No post found with ID {postId}. Please try again.");
                }
                catch (Exception)
                {
                    Console.WriteLine($"Error: Post ID {postId} does not exist in the system. Try again.");
                }
            }
            else
            {
                Console.WriteLine("Error: Please enter a valid number.");
            }
        }
        
        await DisplayCommentsAsync(postId);
        
        Console.Write("\nAdd a comment? y or n?: ");
        string answer = Console.ReadLine();
        if (answer == "y" || answer == "yes")
        {
            await AddCommentAsync(postId);
        }
    }

    // FIXED: Changed 'void' to 'Task' so it can be awaited in ShowPostAsync
    private Task DisplayCommentsAsync(int postId)
    {
        Console.WriteLine("\nComments:");
        
        // FIXED: Removed 'await' because GetMany() is synchronous
        var allComments = commentRepository.GetMany();
        var postComments = allComments.Where(c => c.PostId == postId).ToList();

        if (!postComments.Any())
        {
            Console.WriteLine("  (No comments yet)");
            return Task.CompletedTask;
        }

        foreach (var comment in postComments)
        {
            Console.WriteLine($"  - {comment.Body}");
        }

        return Task.CompletedTask;
    }

    private async Task AddCommentAsync(int postId)
    {
        Console.Write("Comment: ");
        string body = Console.ReadLine();

        int userId = await PromptForValidatingUserIdAsync();

        Comment newComment = new Comment(body, postId, userId);
        
        await commentRepository.AddAsync(newComment);
        Console.WriteLine("Comment added successfully!");
    }

    private async Task<int> PromptForValidatingUserIdAsync()
    {
        while (true)
        {
            Console.Write("Enter your User ID to comment: ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out int userId))
            {
                try
                {
                    User user = await userRepository.GetSingleAsync(userId);
                    if (user != null)
                    {
                        return userId;
                    }
                    Console.WriteLine($"Error: No user found with ID {userId}. Please try again.");
                }
                catch (Exception)
                {
                    Console.WriteLine($"Error: User ID {userId} does not exist in the system. Try again.");
                }
            }
            else
            {
                Console.WriteLine("Error: Please enter a valid number.");
            }
        }
    }
}
