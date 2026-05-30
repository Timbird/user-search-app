using Microsoft.AspNetCore.Mvc;
using UserSearch.Api.Models;
using UserSearch.Api.Services;

namespace UserSearch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet("autocomplete")]
    public async Task<IActionResult> Autocomplete([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            return Ok(Array.Empty<string>());

        var results = await userService.AutocompleteAsync(q.Trim());
        return Ok(results);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Ok(Array.Empty<User>());

        var results = await userService.SearchAsync(q.Trim());
        return Ok(results);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (user, error) = await userService.CreateAsync(request);

        if (error is not null)
            return Conflict(new { message = error });

        return CreatedAtAction(nameof(Search), new { q = user!.FirstName }, user);
    }
}
