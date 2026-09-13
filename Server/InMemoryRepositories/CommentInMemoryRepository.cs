using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class CommentInMemoryRepository : ICommentRepository
{
    private List<Comment> comments = new List<Comment>();
    
    public CommentInMemoryRepository()
    {
        // I add a bunch of dummy data.
        // The underscore is a discard, which means I don't care about the result. AddAsync returns the added comment, but I don't need it here.
        // I call .Result on the Task, because I can't make the constructor async.
        _ = AddAsync(new Comment("Cats are great!", 1, 1)).Result;
        _ = AddAsync(new Comment("So true!", 1, 2)).Result;
        _ = AddAsync(new Comment("They're just so fluffy", 1, 2)).Result;
        _ = AddAsync(new Comment("Mine's hairless!", 1, 1)).Result;
        _ = AddAsync(new Comment("Is it sick?!", 1, 4)).Result;
        
        _ = AddAsync(new Comment("Cats are still great!", 2, 2)).Result;
        _ = AddAsync(new Comment("Man, mine just spat out a dead mouse :(", 2, 3)).Result;
        _ = AddAsync(new Comment("That's a compliment",2, 2)).Result;
        _ = AddAsync(new Comment("No rats around my house!", 2, 1)).Result;

        _ = AddAsync(new Comment("#FIRST", 3, 1)).Result;
        _ = AddAsync(new Comment("They're just so happy and loving", 3, 2)).Result;
        _ = AddAsync(new Comment("Too noisy for me!", 3, 4)).Result;
        _ = AddAsync(new Comment("Uhhh, no?? Cats forever", 3, 4)).Result;
        
        _ = AddAsync(new Comment("Weather is just the greatest!", 4, 4)).Result;
        _ = AddAsync(new Comment("Not today! It's raining :(", 4, 3)).Result;
        _ = AddAsync(new Comment("Rain just smells so nice", 4, 4)).Result;
        _ = AddAsync(new Comment("Weirdo :O", 4, 1)).Result;
        
        _ = AddAsync(new Comment("HELP!?", 5, 1)).Result;
        _ = AddAsync(new Comment("How do I even do anything?", 5, 1)).Result;
        _ = AddAsync(new Comment("I don't understand async", 5, 2)).Result;
        _ = AddAsync(new Comment("What do you need help with?", 5, 3)).Result;
        
        _ = AddAsync(new Comment("Eh, what? Your carpet?", 6, 2)).Result;
        _ = AddAsync(new Comment("I like my Mowinator3000, it just works", 6, 4)).Result;
        _ = AddAsync(new Comment("It just grows out of control!", 6, 3)).Result;
        _ = AddAsync(new Comment("What color is your carpet?", 6, 1)).Result;
    }
    public Task<Comment> AddAsync(Comment comment)
    {
        comment.Id = comments.Any()
            ? comments.Max(c => c.Id) + 1
            : 1;
        comments.Add(comment);
        return Task.FromResult(comment);
    }
    
    public Task UpdateAsync(Comment comment)
    {
        Comment? existingComment = comments.SingleOrDefault(c => c.Id == comment.Id);
        if (existingComment is null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{comment.Id}' not found");
        }

        comments.Remove(existingComment);
        comments.Add(comment);

        return Task.CompletedTask; //Nothing to actually return, so we just return a completed task.
    }
   
    public Task DeleteAsync(int id)
    {
        Comment? commentToRemove = comments.SingleOrDefault(c => c.Id == id);
        if (commentToRemove is null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{id}' not found");
        }

        comments.Remove(commentToRemove);
        return Task.CompletedTask;
    }
   
    public Task<Comment> GetSingleAsync(int id)
    {
        Comment? comment = comments.SingleOrDefault(c => c.Id == id);
        if (comment is null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{id}' not found");
        }
        return Task.FromResult(comment);
    }
   
    public IQueryable<Comment> GetMany()
    {
        return comments.AsQueryable();
    }
   
    
   
}