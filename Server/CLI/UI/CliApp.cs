using RepositoryContracts;
using CLI.UI.ManageUsers;
using CLI.UI.ManagePosts;

namespace CLI.UI;

public class CliApp
{
    private readonly IUserRepository userRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IPostRepository postRepository;

    public CliApp(IUserRepository userRepository, ICommentRepository commentRepository, IPostRepository postRepository)
    {
        this.userRepository = userRepository;
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
    }

    public async Task StartAsync()
    {
        while (true)
        {
            Console.WriteLine("1 --> Manage Users");
            Console.WriteLine("2 --> Manage Posts");
            Console.WriteLine("3 --> Exit Application");
            Console.Write("Select an option: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                ManageUsersView manageUsersView = new ManageUsersView(this.userRepository);
                await manageUsersView.ShowMenuAsync();
            }
            else if (choice == "2")
            {
                ManagePostsView managePostsView = new ManagePostsView(this.postRepository, this.commentRepository, this.userRepository);
                await managePostsView.ShowMenuAsync();
            }
            else if (choice == "3")
            {
                return;
            }
            else
            {
                Console.WriteLine("Please select a valid option (just a numberrrr)");
            }
        }
    }
}