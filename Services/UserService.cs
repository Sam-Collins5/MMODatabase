using Microsoft.AspNetCore.Identity;
using MMOngo.Models;
using MMOngo.Services.Interfaces;
using MongoDB.Driver;

namespace MMOngo.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IPasswordHasher<User> passwordHasher)
        {
            _users = MongoConnection.Database.GetCollection<User>("Users");
            _passwordHasher = passwordHasher;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _users.Find(user => true).ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _users.Find(user => user.Username.ToLower() == username.ToLower()).FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _users.Find(user => user.Email.ToLower() == email.ToLower()).FirstOrDefaultAsync();
        }

        public async Task<User?> RegisterUserAsync(User user, string password)
        {
            User? existingUsername = await GetUserByUsernameAsync(user.Username);

            if (existingUsername != null)
            {
                return null;
            }

            User? existingEmail = await GetUserByEmailAsync(user.Email);

            if (existingEmail != null)
            {
                return null;
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            await _users.InsertOneAsync(user);

            return user;
        }

        public async Task<User?> ValidateLoginAsync(string usernameOrEmail, string password)
        {
            User? user = await _users
                .Find(currentUser =>
                    currentUser.Username.ToLower() == usernameOrEmail.ToLower()
                    || currentUser.Email.ToLower() == usernameOrEmail.ToLower())
                .FirstOrDefaultAsync();

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
    }
}