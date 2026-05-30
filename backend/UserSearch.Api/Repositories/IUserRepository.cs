using UserSearch.Api.Models;

namespace UserSearch.Api.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<string>> AutocompleteAsync(string query);
    Task<IEnumerable<User>> SearchAsync(string query);
    Task<bool> ExistsByEmailAsync(string email);
    Task<User> CreateAsync(User user);
}
