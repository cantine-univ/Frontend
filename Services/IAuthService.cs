using System.Threading.Tasks;

namespace FrontendApp.Services;

public interface IAuthService
{
    Task<string?> LoginAsync(string email, string password);
    Task LogoutAsync();
}
