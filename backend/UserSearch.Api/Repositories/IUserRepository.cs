using UserSearch.Api.Models;

namespace UserSearch.Api.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<string>> AutocompleteAsync(string query);
    Task<(IEnumerable<User> Users, long Total)> SearchAsync(string query, int from);
    Task<bool> ExistsByEmailAsync(string email);
    Task<User> CreateAsync(User user);
}
