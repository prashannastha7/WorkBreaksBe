using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Data;
using WebApi.Models;


namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Register(UserRegistrationDto userRegistrationDto)
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Username == userRegistrationDto.Username))
                return BadRequest("User already exists.");
            if (await _context.Users.AnyAsync(u => u.Email == userRegistrationDto.Email))
                return BadRequest("User already exists.");

            // Create new user
            var newUser = new User
            {
                Username = userRegistrationDto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(userRegistrationDto.Password),
                Email = userRegistrationDto.Email,
                Department = userRegistrationDto.Department,
                Role = UserRole.Employee
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userLoginDto.Username);

            if (user == null)
                return BadRequest("Invalid username or password");

            if (!BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password))
                return BadRequest("Invalid username or password");

            var token = GenerateJwtToken(user);

            return Ok(new { Message = "Login Successful", Token = token, Role = user.Role });
        }

        private string GenerateJwtToken(User user)
        {
            /*
             In this case, the token stores:
            Name(user's username)
            UserId(user's unique identifier)
            Role(the user’s role like Admin or Employee)
            These claims will be part of the token’s payload, which can later be read 
            from the token on the server to validate the user and their role.
             */

            var claims = new[]
            {
                            new Claim("Name", user.Username),
                            new Claim("UserId", user.UserId.ToString()),
                            new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            /*Encoding.UTF8.GetBytes(...): Converts the secret key (from the configuration) into
            a byte array required by the cryptographic library. */
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            //SigningCredentials: These credentials are used to digitally sign the token.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            //WriteToken(token): Converts the JwtSecurityToken object into a compact, serialized string, which is the actual token that will be sent to the user.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        //The Enum.TryParse<>() method allows for case-insensitive conversion
        //by passing true as the second argument.


        [Authorize(Roles = "Employee")]
        [HttpPost("leaves/apply")]
        public async Task<IActionResult> ApplyLeave(LeaveApplicationDto dto)
        {

            //DateOnly.TryParseExact(...) is a static method that attempts to parse a date string into a DateOnly object.
            // Parse StartDate
            if (!DateOnly.TryParseExact(dto.StartDate, "yyyy/MM/dd", out var startDate))
            {
                return BadRequest("Invalid StartDate format. Use yyyy/MM/dd.");
            }

            // Parse EndDate
            if (!DateOnly.TryParseExact(dto.EndDate, "yyyy/MM/dd", out var endDate))
            {
                return BadRequest("Invalid EndDate format. Use yyyy/MM/dd.");
            }

            if (User.IsInRole("Admin"))
            {
                return Forbid("Admin users are not allowed to apply for leave.");
            }

            // Convert LeaveType string to LeaveType enum
            if (!Enum.TryParse<LeaveType>(dto.LeaveType, true, out var leaveType))
                return BadRequest("Invalid leave type. Use 'Paid', 'Sick', 'Maternity', 'Covid', or 'Vacation'.");

            var userId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);

            var leaveApplication = new LeaveApplication
            {
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate,
                LeaveType = leaveType,
                Status = "Pending"
            };
            _context.LeaveApplications.Add(leaveApplication);
            await _context.SaveChangesAsync();

            return Ok("Leave application submitted.");
        }

        [HttpGet("/logout")]
        public async Task<IActionResult> Logout()
        {
            return Ok(new {message = "Logged out successfully" });

        }

    }
}
