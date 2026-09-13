using Entities;
using RepositoryContracts;
namespace CLI.UI.ManageUsers;

public class ListUsersView
{
    private readonly IUserRepository  userRepository;

    public ListUsersView(IUserRepository repository)
    {
        this.userRepository = repository;
    }

    public Task ListUsersAsync()
    {
        Console.WriteLine("\nRegistered Users");

        IQueryable<User> users = userRepository.GetMany();

        foreach (User user in users)
        {
            Console.WriteLine($" ID: {user.Id}   Username: {user.Username}");
        }
        return Task.CompletedTask;
        //No await because GetMany() is synchronous so no asyc Task so completed.task
    }
}