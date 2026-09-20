using CLI.UI;
using FileRepositories;
using RepositoryContracts;


Console.WriteLine("Starting Command Line Interface (CLI) app...");
IUserRepository userRepository = new UserFileRepository(); //old UserInMemoryRepository();
ICommentRepository commentRepository = new CommentFileRepository();
IPostRepository postRepository = new PostFileRepository();

CliApp cliApp = new CliApp(userRepository, commentRepository, postRepository);
await cliApp.StartAsync();