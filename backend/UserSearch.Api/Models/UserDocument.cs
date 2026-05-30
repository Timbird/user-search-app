namespace UserSearch.Api.Models;

public class UserDocument
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public User ToUser() => new()
    {
        Id = Id,
        FirstName = FirstName,
        LastName = LastName,
        JobTitle = JobTitle,
        Phone = Phone,
        Email = Email
    };

    public static UserDocument FromUser(User user) => new()
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        FullName = $"{user.FirstName} {user.LastName}",
        JobTitle = user.JobTitle,
        Phone = user.Phone,
        Email = user.Email
    };
}
