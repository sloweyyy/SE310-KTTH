using Microsoft.AspNetCore.Mvc;
using ApiAdminDashboard.Data.Models;
using ApiAdminDashboard.Data;
using ApiAdminDashboard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace ApiAdminDashboard.Controllers
{
  [Route("api/v1/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;

    public AuthController(IConfiguration configuration, ApplicationDbContext context)
    {
      _configuration = configuration;
      _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterModel register)
    {
      if (await _context.Users.AnyAsync(u => u.Email == register.Email))
      {
        return BadRequest("Email already exists");
      }

      CreatePasswordHash(register.Password, out byte[] passwordHash, out byte[] passwordSalt);

      var user = new User
      {
        Email = register.Email,
        PasswordHash = passwordHash,
        PasswordSalt = passwordSalt,
        Role = register.Role
      };

      try
      {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok(user);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error saving user to database: {ex.Message}");
        return StatusCode(500, "Internal server error");
      }
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel login)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == login.Email);

      if (user == null)
      {
        return Unauthorized("Invalid Email");
      }

      if (!VerifyPasswordHash(login.Password, user.PasswordHash, user.PasswordSalt))
      {
        return Unauthorized("Invalid password");
      }

      string token = CreateToken(user);
      string refreshToken = CreateRefreshToken();

      var refreshTokenModel = new RefreshToken
      {
        Token = refreshToken,
        UserId = user.Id.ToString(),
        ExpiryDate = DateTime.Now.AddDays(7),
        IsRevoked = false
      };

      _context.RefreshTokens.Add(refreshTokenModel);
      await _context.SaveChangesAsync();

      return Ok(new
      {
        token,
        refreshToken,
        role = user.Role
      });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(string token, string refreshToken)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == GetUserIdFromJwt(token));

      if (user == null)
      {
        return Unauthorized("Invalid token");
      }

      var refreshTokenModel = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);

      if (refreshTokenModel == null || refreshTokenModel.IsRevoked || refreshTokenModel.ExpiryDate < DateTime.Now)
      {
        return Unauthorized("Invalid refresh token");
      }

      string newToken = CreateToken(user);
      string newRefreshToken = CreateRefreshToken();

      refreshTokenModel.Token = newRefreshToken;
      refreshTokenModel.ExpiryDate = DateTime.Now.AddDays(7);

      _context.RefreshTokens.Update(refreshTokenModel);
      await _context.SaveChangesAsync();

      return Ok(new
      {
        token = newToken,
        refreshToken = newRefreshToken
      });
    }

    [HttpPost("revoke-token")]
    [Authorize]
    public async Task<IActionResult> RevokeToken(string refreshToken)
    {
      var refreshTokenModel = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);

      if (refreshTokenModel == null)
      {
        return NotFound("Refresh token not found");
      }

      refreshTokenModel.IsRevoked = true;
      _context.RefreshTokens.Update(refreshTokenModel);
      await _context.SaveChangesAsync();

      return Ok("Refresh token revoked");
    }

    private string CreateToken(User user)
    {
      List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

      var token = new JwtSecurityToken(
          claims: claims,
          expires: DateTime.Now.AddMinutes(30),
          signingCredentials: creds
      );

      var jwt = new JwtSecurityTokenHandler().WriteToken(token);

      return jwt;
    }

    private string CreateRefreshToken()
    {
      var randomNumber = new byte[32];
      using var rng = RandomNumberGenerator.Create();
      rng.GetBytes(randomNumber);
      return Convert.ToBase64String(randomNumber);
    }

    private string GetUserIdFromJwt(string token)
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

      try
      {
        var tokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = key,
          ValidateIssuer = false,
          ValidateAudience = false,
          ClockSkew = TimeSpan.Zero
        };

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        return principal.FindFirstValue(ClaimTypes.NameIdentifier);
      }
      catch (Exception ex)
      {
        return null;
      }
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
      using var hmac = new HMACSHA512();
      passwordSalt = hmac.Key;
      passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
      using var hmac = new HMACSHA512(passwordSalt);
      var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
      return computedHash.SequenceEqual(passwordHash);
    }
  }
}