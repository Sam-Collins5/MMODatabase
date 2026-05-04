using MMOngo.Models;

namespace MMOngo.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();

        Task<User?> GetUserByIdAsync(string id);

        Task<User?> GetUserByUsernameAsync(string username);

        Task<User?> GetUserByEmailAsync(string email);

        Task<User?> RegisterUserAsync(User user, string password);

        Task<User?> ValidateLoginAsync(string usernameOrEmail, string password);
    }
}