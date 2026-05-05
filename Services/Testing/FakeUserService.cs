using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using MMOngo.Models;
using MMOngo.Services.Interfaces;

namespace MMOngo.Services
{
    public class FakeUserService : IUserService
    {
        private readonly IPasswordHasher<User> _passwordHasher;

        private static readonly string DataFolderPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Data"
        );

        private static readonly string TempUserFilePath = Path.Combine(
            DataFolderPath,
            "temp_users.json"
        );

        public FakeUserService(IPasswordHasher<User> passwordHasher)
        {
            _passwordHasher = passwordHasher;

            if (!Directory.Exists(DataFolderPath))
            {
                Directory.CreateDirectory(DataFolderPath);
            }

            if (!File.Exists(TempUserFilePath))
            {
                File.WriteAllText(TempUserFilePath, "[]");
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await LoadUsersAsync();
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            List<User> users = await LoadUsersAsync();

            return users.FirstOrDefault(user => user.Id == id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            List<User> users = await LoadUsersAsync();

            return users.FirstOrDefault(user =>
                user.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            List<User> users = await LoadUsersAsync();

            return users.FirstOrDefault(user =>
                user.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<User?> RegisterUserAsync(User user, string password)
        {
            List<User> users = await LoadUsersAsync();

            bool usernameExists = users.Any(currentUser =>
                currentUser.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase));

            if (usernameExists)
            {
                return null;
            }

            bool emailExists = users.Any(currentUser =>
                currentUser.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase));

            if (emailExists)
            {
                return null;
            }

            user.Id = Guid.NewGuid().ToString();
            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            users.Add(user);

            await SaveUsersAsync(users);

            return user;
        }

        public async Task<User?> ValidateLoginAsync(string usernameOrEmail, string password)
        {
            List<User> users = await LoadUsersAsync();

            User? user = users.FirstOrDefault(currentUser =>
                currentUser.Username.Equals(usernameOrEmail, StringComparison.OrdinalIgnoreCase)
                || currentUser.Email.Equals(usernameOrEmail, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                return null;
            }

            PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password
            );

            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return user;
        }

        private static async Task<List<User>> LoadUsersAsync()
        {
            if (!Directory.Exists(DataFolderPath))
            {
                Directory.CreateDirectory(DataFolderPath);
            }

            if (!File.Exists(TempUserFilePath))
            {
                await File.WriteAllTextAsync(TempUserFilePath, "[]");
            }

            string json = await File.ReadAllTextAsync(TempUserFilePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<User>();
            }

            List<User>? users = JsonSerializer.Deserialize<List<User>>(json);

            return users ?? new List<User>();
        }

        private static async Task SaveUsersAsync(List<User> users)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(users, options);

            await File.WriteAllTextAsync(TempUserFilePath, json);
        }
    }
}