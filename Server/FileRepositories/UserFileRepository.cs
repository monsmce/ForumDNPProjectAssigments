using System.IO;
using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;


public class UserFileRepository : IUserRepository
{
    private readonly string filePath = "user.json";//Here we define the file path. We create a file per entity.
   
    
    public UserFileRepository()
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

    private async Task<List<User>> LoadUsersAsync() 
        //Helper methods (reads the file, deserialize, returns the list of users)
    {
        string json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
    }
    
    private async Task SaveUsersAsync(List<User> users)
        //Serializes the provided list and writes it back to file
    {
        string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }
    
    
    
    public async Task<User> AddAsync(User user) //his is the method header, asynchronous, returning a Task containing the “finalized” user, i.e., it now has an ID
    {
        //use helper to load users, removing code duplication
        List<User> users = await LoadUsersAsync(); 
        
        int maxId = users.Count > 0 ? users.Max(u => u.Id) : 0; 
        user.Id = maxId + 1;
        
        users.Add(user); 
        
        // We use the helper to save the changes.
        await SaveUsersAsync(users); 
        
        return user; 
        
        
        /*string usersAsJson = await File.ReadAllTextAsync(filePath); // We read all the content from the file; this is, of course, in JSON format.
        List<User> users = JsonSerializer.Deserialize<List<User>>(usersAsJson) ?? new List<User>(); //The JSON is deserialized into a list of users.
        int maxId = users.Count > 0 ? users.Max(u => u.Id) : 0; // We calculate the next ID to use.
        user.Id = maxId + 1;//Set the ID.
        users.Add(user); //Add the user to the list.
        usersAsJson = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true }); //Serialize the list to JSON.
        await File.WriteAllTextAsync(filePath, usersAsJson); // Write the JSON back to the file.
        
        return user; //Return the finalized user now that it has an ID.*/
    }
    
   
    
    
    
    public async Task UpdateAsync(User user)
    {
        List<User> users = await LoadUsersAsync();

        User? existingUser = users.SingleOrDefault(u => u.Id == user.Id);
        if (existingUser is null)
        {
            throw new InvalidOperationException($"User with ID '{user.Id}' not found");
        }

        users.Remove(existingUser);
        users.Add(user);

        await SaveUsersAsync(users);
    }

    public async Task DeleteAsync(int id)
    {
        List<User> users = await LoadUsersAsync();
        
        var userToRemove = users.SingleOrDefault(u => u.Id == id);
        if (userToRemove is null)
        {
            throw new InvalidOperationException($"User with ID '{id}' not found");
        }

        users.Remove(userToRemove);
        await SaveUsersAsync(users);
    }

    public async Task<User> GetSingleAsync(int id)
    {
        List<User> users = await LoadUsersAsync();
        
        User? user = users.SingleOrDefault(u => u.Id == id);
        if (user is null)
        {
            throw new InvalidOperationException($"User with ID '{id}' not found");
        }

       //return Task.FromResult(user); the methoid is async,so plain object it returnesd
       return user;
    }

    public IQueryable<User> GetMany()
    {
        /*string usersAsJson = File.ReadAllTextAsync(filePath).Result; //ReadAllTextAsync() returns a Task containing a string calling result instead of awaiting
        List<User> users = JsonSerializer.Deserialize<List<User>>(usersAsJson)!;
        return users.AsQueryable();*/
        
        List<User> users = LoadUsersAsync().Result; //Result instead of awaiting will extract the string. This is generally not advisable because we lose the async behavior and optimization, but in this case, the method header is not async as defined in the interface, so we have to “cheat.”
        return users.AsQueryable();
        //previous version repeats the load logic and ends with !, which would crash if the file ever contained null.
        //Helper (LoadUsersAsync()) handles that case.Result would safer and shorter.
    }
}
