using Moq;
using Xunit;
using UserSearch.Api.Models;
using UserSearch.Api.Repositories;
using UserSearch.Api.Services;

namespace UserSearch.Api.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repoMock = new();
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _sut = new UserService(_repoMock.Object);
    }

    // ── AutocompleteAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task AutocompleteAsync_DelegatesToRepository()
    {
        var expected = new[] { "Phil Walker", "Phillipa Walker" };
        _repoMock.Setup(r => r.AutocompleteAsync("phi")).ReturnsAsync(expected);

        var result = await _sut.AutocompleteAsync("phi");

        Assert.Equal(expected, result);
    }

    // ── SearchAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task SearchAsync_ReturnsUsersFromRepository()
    {
        var users = new List<User>
        {
            new() { Id = "1", FirstName = "Phil", LastName = "Walker", JobTitle = "Senior QA", Phone = "07889 984447", Email = "pwalker@test.com" }
        };
        _repoMock.Setup(r => r.SearchAsync("walker", 0)).ReturnsAsync((users, 1L));

        var (result, total) = await _sut.SearchAsync("walker", 0);

        Assert.Single(result);
        Assert.Equal("Phil", result.First().FirstName);
        Assert.Equal(1L, total);
    }

    [Fact]
    public async Task SearchAsync_PassesFromToRepository()
    {
        _repoMock.Setup(r => r.SearchAsync("walker", 25)).ReturnsAsync((new List<User>(), 30L));

        var (_, total) = await _sut.SearchAsync("walker", 25);

        _repoMock.Verify(r => r.SearchAsync("walker", 25), Times.Once);
        Assert.Equal(30L, total);
    }

    // ── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_HappyPath_ReturnsCreatedUser()
    {
        var request = new CreateUserRequest
        {
            FirstName = "Susan",
            LastName = "Kim",
            JobTitle = "Developer",
            Phone = "07775 357959",
            Email = "skim@test.com"
        };
        _repoMock.Setup(r => r.ExistsByEmailAsync("skim@test.com")).ReturnsAsync(false);
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<User>()))
                 .ReturnsAsync((User u) => { u.Id = "new-id"; return u; });

        var (user, error) = await _sut.CreateAsync(request);

        Assert.Null(error);
        Assert.NotNull(user);
        Assert.Equal("Susan", user.FirstName);
        Assert.Equal("skim@test.com", user.Email);
    }

    [Fact]
    public async Task CreateAsync_DuplicateEmail_ReturnsError()
    {
        var request = new CreateUserRequest
        {
            FirstName = "Phil",
            LastName = "Walker",
            JobTitle = "Senior QA",
            Phone = "07889 984447",
            Email = "pwalker@test.com"
        };
        _repoMock.Setup(r => r.ExistsByEmailAsync("pwalker@test.com")).ReturnsAsync(true);

        var (user, error) = await _sut.CreateAsync(request);

        Assert.Null(user);
        Assert.NotNull(error);
        Assert.Contains("already exists", error);
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_NormalisesEmailToLowercase()
    {
        var request = new CreateUserRequest
        {
            FirstName = "New",
            LastName = "User",
            JobTitle = "Developer",
            Phone = "07789 543768",
            Email = "NewUser@Test.COM"
        };
        _repoMock.Setup(r => r.ExistsByEmailAsync("newuser@test.com")).ReturnsAsync(false);
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<User>()))
                 .ReturnsAsync((User u) => u);

        var (user, _) = await _sut.CreateAsync(request);

        Assert.Equal("newuser@test.com", user!.Email);
    }

    [Fact]
    public async Task CreateAsync_TrimsWhitespaceFromFields()
    {
        var request = new CreateUserRequest
        {
            FirstName = "  Anna  ",
            LastName = "  Bell  ",
            JobTitle = "  Developer  ",
            Phone = "07789 543768",
            Email = "  abell@test.com  "
        };
        _repoMock.Setup(r => r.ExistsByEmailAsync("abell@test.com")).ReturnsAsync(false);
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<User>()))
                 .ReturnsAsync((User u) => u);

        var (user, _) = await _sut.CreateAsync(request);

        Assert.Equal("Anna", user!.FirstName);
        Assert.Equal("Bell", user.LastName);
        Assert.Equal("abell@test.com", user.Email);
    }
}
