using FrontendApp.Models;
using System.Threading.Tasks;

namespace FrontendApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApiService _api;
        private readonly StorageService _storage;

        public AuthService(ApiService api, StorageService storage)
        {
            _api = api;
            _storage = storage;
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var loginDto = new UserLoginDto
            {
                Email = email,
                Password = password
            };

            var authResponse = await _api.LoginAsync(loginDto);

            if (authResponse != null && !string.IsNullOrEmpty(authResponse.Token) && !string.IsNullOrEmpty(authResponse.UserId))
            {
                await _storage.SetAsync("auth_token", authResponse.Token);
                await _storage.SetAsync("user_id", authResponse.UserId); 

                await _storage.SetAsync("role", authResponse.Role); 

                return authResponse.Token; 
            }

            return null;
        }

        public async Task LogoutAsync()
        {
            await _storage.RemoveAsync("auth_token");
            await _storage.RemoveAsync("user_id"); 
            await _storage.RemoveAsync("role");
        }
    }
}