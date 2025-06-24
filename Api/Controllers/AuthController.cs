using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Api.Domain.Entities;
using Api.Infrastructure.Repositories;
using System.Linq;
using Api.Application.Common.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        // GET: api/auth/session
        [HttpGet("session")]
        public async Task<IActionResult> GetSession()
        {
            // Extract user from JWT claims
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Ok(new { user = (object?)null });

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Ok(new { user = (object?)null });

            return Ok(new
            {
                user = new
                {
                    id = user.Id,
                    handle = user.Handle ?? string.Empty,
                    name = user.Name ?? string.Empty,
                    email = user.Email ?? string.Empty,
                    image = user.Image ?? string.Empty
                }
            });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Email and password are required." });
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                    return Unauthorized(new { message = "Invalid credentials" });
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (!result.Succeeded)
                    return Unauthorized(new { message = "Invalid credentials" });
                var jwtKey = _config["Jwt:Key"]!;
                var jwtIssuer = _config["Jwt:Issuer"]!;
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                    new Claim("name", user.Name ?? string.Empty),
                    new Claim("handle", user.Handle ?? string.Empty)
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: null,
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(7),
                    signingCredentials: creds
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(new { token = tokenString });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login.", error = ex.Message });
            }
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "Email, password, and name are required." });
            try
            {
                var existing = await _userManager.FindByEmailAsync(request.Email);
                if (existing != null)
                    return Conflict(new { message = "A user with this email already exists." });
                var user = new User
                {
                    UserName = request.Email,
                    Email = request.Email,
                    Name = request.Name,
                    Handle = request.Handle,
                    Image = request.Image,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
                }
                var jwtKey = _config["Jwt:Key"]!;
                var jwtIssuer = _config["Jwt:Issuer"]!;
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                    new Claim("name", user.Name ?? string.Empty),
                    new Claim("handle", user.Handle ?? string.Empty)
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: null,
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(7),
                    signingCredentials: creds
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(new { token = tokenString });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration.", error = ex.Message });
            }
        }

        // POST: api/auth/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // For JWT, logout is handled client-side by deleting the token.
            return Ok(new { message = "Logged out" });
        }

        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class RegisterRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Handle { get; set; } = string.Empty;
            public string? Image { get; set; }
        }
    }
}
