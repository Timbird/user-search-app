using System.Net;
using System.Net.Http.Json;
using UserSearch.Api.IntegrationTests.Infrastructure;
using UserSearch.Api.Models;

namespace UserSearch.Api.IntegrationTests.Controllers;

[Collection("Integration")]
public class UsersControllerTests
{
    private readonly HttpClient _client;
    private readonly IntegrationTestFactory _factory;

    public UsersControllerTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    // --- Autocomplete ---

    [Fact]
    public async Task Autocomplete_QueryTooShort_ReturnsEmptyArray()
    {
        var response = await _client.GetAsync("/api/users/autocomplete?q=w");

        response.EnsureSuccessStatusCode();
        var results = await response.Content.ReadFromJsonAsync<string[]>();
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    [Fact]
    public async Task Autocomplete_WithWalkerPrefix_ReturnsWalkerSuggestions()
    {
        var response = await _client.GetAsync("/api/users/autocomplete?q=wa");

        response.EnsureSuccessStatusCode();
        var results = await response.Content.ReadFromJsonAsync<string[]>();
        Assert.NotNull(results);
        Assert.NotEmpty(results);
        // All returned suggestions must contain "walker" — seeded data has Walker/Walker-Smith
        Assert.All(results, s => Assert.Contains("walker", s, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Autocomplete_NoMatch_ReturnsEmptyArray()
    {
        var response = await _client.GetAsync("/api/users/autocomplete?q=zzz");

        response.EnsureSuccessStatusCode();
        var results = await response.Content.ReadFromJsonAsync<string[]>();
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    // --- Search ---

    [Fact]
    public async Task Search_EmptyQuery_ReturnsEmptyArray()
    {
        var response = await _client.GetAsync("/api/users/search?q=");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<SearchResponse>();
        Assert.NotNull(body);
        Assert.Empty(body.Users);
        Assert.Equal(0, body.Total);
    }

    [Fact]
    public async Task Search_WithSmith_ReturnsSmithUsers()
    {
        var response = await _client.GetAsync("/api/users/search?q=smith");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<SearchResponse>();
        Assert.NotNull(body);
        Assert.NotEmpty(body.Users);
        // Seeded: Alex Smith, Kathy Smith, Hayley Walker-Smith
        Assert.Contains(body.Users, u =>
            u.LastName.Contains("Smith", StringComparison.OrdinalIgnoreCase) ||
            u.FirstName.Contains("Smith", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Search_NoMatch_ReturnsEmptyArray()
    {
        var response = await _client.GetAsync("/api/users/search?q=zzznomatch");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<SearchResponse>();
        Assert.NotNull(body);
        Assert.Empty(body.Users);
        Assert.Equal(0, body.Total);
    }

    private record SearchResponse(User[] Users, long Total);

    // --- Create ---

    [Fact]
    public async Task Create_ValidRequest_Returns201WithUser()
    {
        var request = new
        {
            firstName = "Integration",
            lastName = "Test",
            jobTitle = "QA Engineer",
            phone = "07123 456789",
            email = $"integration.{Guid.NewGuid():N}@example.com"
        };

        var response = await _client.PostAsJsonAsync("/api/users", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(created);
        Assert.Equal("Integration", created.FirstName);
        Assert.Equal("Test", created.LastName);
        Assert.False(string.IsNullOrEmpty(created.Id));
    }

    [Fact]
    public async Task Create_DuplicateEmail_Returns409()
    {
        var email = $"duplicate.{Guid.NewGuid():N}@example.com";
        var request = new
        {
            firstName = "First",
            lastName = "User",
            jobTitle = "Tester",
            phone = "07123 456789",
            email
        };

        var first = await _client.PostAsJsonAsync("/api/users", request);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        // Force a refresh so the duplicate check can find the first user
        await _factory.ForceRefreshAsync();

        var second = await _client.PostAsJsonAsync("/api/users", request);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Create_InvalidPhone_Returns400()
    {
        var request = new
        {
            firstName = "Bad",
            lastName = "Phone",
            jobTitle = "Tester",
            phone = "not-a-valid-number",
            email = "badphone@example.com"
        };

        var response = await _client.PostAsJsonAsync("/api/users", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_MissingRequiredFields_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/users", new { firstName = "OnlyFirst" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
