using Entities;
using RepositoryContracts;
namespace CLI.UI.ManagePosts;

public class CreatePostView
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;

    public CreatePostView(IPostRepository postRepository, IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
    }

    public async Task CreatePostAsync()
    {
        Console.WriteLine("\nCreate New Post");

        string title = PromptForTitle();
        string body = PromptForBody();
        int userId = await PromptForValidatingUserIdAsync();

        await SavePostAsync(title, body, userId);
    }

    private string PromptForTitle()
    {
        while (true)
        {
            Console.Write("Enter title : ");
            string input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            Console.WriteLine("Error: Hey write a title whats wrong with you");
        }
    }

    private string PromptForBody()
    { 
        Console.Write("Write the Body: "); 
        return Console.ReadLine();
        
    }
    

    private async Task<int> PromptForValidatingUserIdAsync()
    {
        while (true)
        {
            Console.Write("Enter user ID: ");
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

    private async Task SavePostAsync(string title, string body, int userId)
    {
        Post newPost = new Post(title, body, userId);
        await postRepository.AddAsync(newPost);

        Console.WriteLine($"\nSuccess: Post '{title}' has been successfully created!");
        
    }
    
}


