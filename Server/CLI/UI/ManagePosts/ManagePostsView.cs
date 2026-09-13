using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class ManagePostsView
{
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;

    public ManagePostsView(IPostRepository postRepository, ICommentRepository commentRepository, IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
    }

    public async Task ShowMenuAsync()
    {
        while (true)
        {
            Console.WriteLine("1 --> Create Post");
            Console.WriteLine("2 --> List Posts");
            Console.WriteLine("3 --> View Single Post");
            Console.WriteLine("4 --> Go Back");
            Console.Write("Select an option: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                CreatePostView createPostView = new CreatePostView(this.postRepository, this.userRepository);
                await createPostView.CreatePostAsync();
            }
            else if (choice == "2")
            {
                ListPostsView listPostsView = new ListPostsView(this.postRepository);
                await listPostsView.ListPostsAsync();
            }
            else if (choice == "3")
            {
                SinglePostView singlePostView = new SinglePostView(this.postRepository, this.commentRepository, this.userRepository);
                await singlePostView.ShowPostAsync(); 
            }
            else if (choice == "4")
            {
                return;
            }
            else
            {
                Console.WriteLine("Please select a valid option (just a number idiot)");
            }
        }
    }
}