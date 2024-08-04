using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FinancialPlanningAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinancialPlanningAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetUserInfo()
    {
        var usernameClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (usernameClaim == null)
        {
            return Unauthorized("Invalid token.");
        }

        var username = usernameClaim.Value;
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized("Invalid token.");
        }

        var user = await context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        var userInfo = new
        {
            user.Username,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        };

        return Ok(userInfo);
    }
}