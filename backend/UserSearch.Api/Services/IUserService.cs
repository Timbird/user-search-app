using UserSearch.Api.Models;

namespace UserSearch.Api.Services;

public interface IUserService
{
    Task<IEnumerable<string>> AutocompleteAsync(string query);
    Task<IEnumerable<User>> SearchAsync(string query);
    Task<(User? user, string? error)> CreateAsync(CreateUserRequest request);
}
