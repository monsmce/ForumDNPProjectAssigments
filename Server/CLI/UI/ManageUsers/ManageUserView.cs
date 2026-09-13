using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class ManageUsersView
{
    private readonly IUserRepository userRepository;

    public ManageUsersView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task ShowMenuAsync()
    {
        while (true)
        {
            Console.WriteLine("1 --> Create User");
            Console.WriteLine("2 --> List Users");
            Console.WriteLine("3 --> Go Back");
            Console.Write("Select an option: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                CreateUserView createUserView = new CreateUserView(this.userRepository);
                await createUserView.CreateUserAsync();
            }

            else if (choice == "2")
            {
                ListUsersView listUsersView = new ListUsersView(this.userRepository);
                await listUsersView.ListUsersAsync();
            }

            else if (choice == "3")
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
