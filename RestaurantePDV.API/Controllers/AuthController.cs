using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestaurantePDV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        // Validação simples (em produção, validar contra banco de dados com funcionários)
        var userInfo = GetUserInfo(request.Username, request.Password);
        if (userInfo != null)
        {
            var token = GenerateJwtToken(userInfo);
            return Ok(new LoginResponse
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(24),
                Username = userInfo.Username,
                Role = userInfo.Role,
                NivelAcesso = userInfo.NivelAcesso
            });
        }

        return Unauthorized("Credenciais inválidas");
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var principal = GetPrincipalFromExpiredToken(request.Token);
            var username = principal?.Identity?.Name;
            var role = principal?.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
                return Unauthorized("Token inválido");

            var userInfo = new UserInfo
            {
                Username = username,
                Role = role,
                NivelAcesso = GetNivelAcessoFromRole(role)
            };

            var newToken = GenerateJwtToken(userInfo);
            return Ok(new LoginResponse
            {
                Token = newToken,
                Expiration = DateTime.UtcNow.AddHours(24),
                Username = username,
                Role = role,
                NivelAcesso = userInfo.NivelAcesso
            });
        }
        catch
        {
            return Unauthorized("Token inválido");
        }
    }

    private UserInfo? GetUserInfo(string username, string password)
    {
        // Implementação simples para demonstração
        // Em produção, validar contra banco de dados de funcionários
        return username.ToLower() switch
        {
            "admin" when password == "123456" => new UserInfo
            {
                Username = "admin",
                Role = "Admin",
                NivelAcesso = "Administrador",
                FuncionarioId = 1
            },
            "gerente" when password == "123456" => new UserInfo
            {
                Username = "gerente",
                Role = "Gerente",
                NivelAcesso = "Gerente",
                FuncionarioId = 2
            },
            "operador" when password == "123456" => new UserInfo
            {
                Username = "operador",
                Role = "UsuarioComum",
                NivelAcesso = "Usuário Comum",
                FuncionarioId = 3
            },
            "garcom" when password == "123456" => new UserInfo
            {
                Username = "garcom",
                Role = "UsuarioComum",
                NivelAcesso = "Usuário Comum",
                FuncionarioId = 4
            },
            _ => null
        };
    }

    private string GetNivelAcessoFromRole(string role)
    {
        return role switch
        {
            "Admin" => "Administrador",
            "Gerente" => "Gerente",
            "UsuarioComum" => "Usuário Comum",
            _ => "Usuário Comum"
        };
    }

    private string GenerateJwtToken(UserInfo userInfo)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userInfo.Username),
            new Claim(ClaimTypes.NameIdentifier, userInfo.FuncionarioId.ToString()),
            new Claim(ClaimTypes.Role, userInfo.Role),
            new Claim("nivel_acesso", userInfo.NivelAcesso),
            new Claim("funcionario_id", userInfo.FuncionarioId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
            ValidateLifetime = false // Não validar expiração para refresh
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Token inválido");
        }

        return principal;
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string NivelAcesso { get; set; } = string.Empty;
}

public class RefreshTokenRequest
{
    public string Token { get; set; } = string.Empty;
}

public class UserInfo
{
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string NivelAcesso { get; set; } = string.Empty;
    public int FuncionarioId { get; set; }
}