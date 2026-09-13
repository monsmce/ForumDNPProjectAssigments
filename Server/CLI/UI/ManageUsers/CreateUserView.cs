using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class CreateUserView
{
    private readonly IUserRepository userRepository;
    
    public CreateUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }
        
    public async Task CreateUserAsync()
    {
        Console.WriteLine("\n--- Create New User ---");
        
        string username = await PromptForUsername();
        string password = PromptForPassword();
        
        await SaveUserAsync(username, password);
    }

   
    private Task<string> PromptForUsername()
    {
        while (true)
        {
            Console.Write("Enter username: ");
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Error: Username cannot be empty. Please try again.");
                continue;
            }

            var allUsers = userRepository.GetMany();
            bool usernameExists = allUsers.Any(u => u.Username.Equals(input));

            if (usernameExists)
            {
                Console.WriteLine($"Error: Username '{input}' is already taken. Please try again.");
                continue;
            }

            return Task.FromResult(input);
        }
    }

    private string PromptForPassword()
    {
        while (true)
        {
            Console.Write("Enter password: ");
            string input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            Console.WriteLine("Error: Password cannot be empty. Please try again.");
        }
    }

    private async Task SaveUserAsync(string username, string password)
    {
        User newUser = new User(username, password);
        
        await userRepository.AddAsync(newUser);

        Console.WriteLine($"User {newUser.Username} created, with ID: {newUser.Id} assigned");
    }
}
