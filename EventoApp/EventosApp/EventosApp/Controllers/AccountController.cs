using EventosApp.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly DbHelper _db;
    private readonly IConfiguration _config;

    public AccountController(DbHelper db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Username) ||
            string.IsNullOrWhiteSpace(req.Password))
            return BadRequest("Usuario y contraseña requeridos");

        bool isAdmin = false;

        if (req.IsAdmin)
        {
            if (string.IsNullOrWhiteSpace(req.AdminSecret) ||
                req.AdminSecret != _config["AdminRegistrationSecret"])
                return BadRequest("Secreto de administrador inválido");

            isAdmin = true;
        }

        using var conn = _db.GetConnection();
        using var cmd = new SqlCommand("usp_RegisterUser", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@Username", req.Username);
        cmd.Parameters.AddWithValue("@Password", req.Password);
        cmd.Parameters.AddWithValue("@IsAdmin", isAdmin);

        try
        {
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        catch (SqlException ex) when (ex.Number == 2627)
        {
            return BadRequest("El usuario ya existe");
        }

        return Ok();
    }

    [Authorize]
    [HttpGet("debug/claims")]
    public IActionResult DebugClaims()
    {
        return Ok(User.Claims.Select(c => new {
            c.Type,
            c.Value
        }));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        return Ok();
    }



    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        using var conn = _db.GetConnection();
        using var cmd = new SqlCommand("usp_GetUserPlain", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@Username", req.Username);
        cmd.Parameters.AddWithValue("@Password", req.Password);

        conn.Open();
        using var reader = cmd.ExecuteReader();

        if (!reader.Read())
            return Unauthorized("Credenciales incorrectas");

        var userId = reader["Id"].ToString();
        var isAdmin = (bool)reader["IsAdmin"];

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, req.Username),
            new Claim("UserId", userId)
        };

        if (isAdmin)
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity)
        );

        return Ok(new
        {
            username = req.Username,
            isAdmin = isAdmin
        });


    }
}

public record RegisterRequest(string Username, string Password, bool IsAdmin, string? AdminSecret);
public record LoginRequest(string Username, string Password);
