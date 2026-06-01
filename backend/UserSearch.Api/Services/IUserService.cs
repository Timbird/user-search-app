using UserSearch.Api.Models;

namespace UserSearch.Api.Services;

public interface IUserService
{
    Task<IEnumerable<string>> AutocompleteAsync(string query);
    Task<(IEnumerable<User> Users, long Total)> SearchAsync(string query, int from);
    Task<(User? user, string? error)> CreateAsync(CreateUserRequest request);
}
