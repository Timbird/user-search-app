using UserSearch.Api.Models;
using UserSearch.Api.Repositories;

namespace UserSearch.Api.Services;

public class UserService(IUserRepository repository) : IUserService
{
    public Task<IEnumerable<string>> AutocompleteAsync(string query) =>
        repository.AutocompleteAsync(query);

    public Task<(IEnumerable<User> Users, long Total)> SearchAsync(string query, int from) =>
        repository.SearchAsync(query, from);

    public async Task<(User? user, string? error)> CreateAsync(CreateUserRequest request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        if (await repository.ExistsByEmailAsync(normalizedEmail))
            return (null, "A user with this email address already exists.");

        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            JobTitle = request.JobTitle.Trim(),
            Phone = request.Phone.Trim(),
            Email = normalizedEmail
        };

        var created = await repository.CreateAsync(user);
        return (created, null);
    }
}
