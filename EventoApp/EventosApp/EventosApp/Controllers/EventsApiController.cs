using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventosApp.Data;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

[ApiController]
[Route("api/events")]
public class EventsApiController : ControllerBase
{
    private readonly DbHelper _db;
    public EventsApiController(DbHelper db) { _db = db; }

    [HttpGet("list")]
    public IActionResult List()
    {
        var list = new List<object>();
        using var conn = _db.GetConnection();
        using var cmd = new SqlCommand("usp_ListEvents", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        conn.Open();
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new
            {
                id = r["Id"],
                title = r["Titulo"],
                description = r["Descripcion"],
                tipo = r["Tipo"],
                localizacion = r["Localizacion"],
                startDate = r["Inicio"],
                finalDate = r["Final"],
                inscritos = r["Inscritos"]
            });
        }
        return Ok(list);
    }

    [Authorize]
    [HttpPost("register")]
    public IActionResult Register(RegisterEventRequest req)
    {
        var userId = int.Parse(
            User.Claims.First(c => c.Type == "UserId").Value
        );

        using var conn = _db.GetConnection();
        using var cmd = new SqlCommand("usp_RegisterToEvent", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@EventoId", req.EventoId);
        cmd.Parameters.AddWithValue("@UsuarioId", userId);

        conn.Open();
        cmd.ExecuteNonQuery();

        return Ok();
    }


    [Authorize]
    [HttpGet("is-registered/{eventId}")]
    public IActionResult IsRegistered(int eventId)
    {
        int userId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);

        using var conn = _db.GetConnection();
        using var cmd = new SqlCommand("usp_IsUserRegistered", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@UsuarioId", userId);
        cmd.Parameters.AddWithValue("@EventoId", eventId);

        conn.Open();
        int count = (int)cmd.ExecuteScalar();

        return Ok(new { isRegistered = count > 0 });
    }

    [Authorize]
    [HttpPost("cancel")]
    public IActionResult CancelRegistration(RegisterEventRequest req)
    {
        int userId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);

        using var conn = _db.GetConnection();
        using var cmd = new SqlCommand("usp_CancelRegistration", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@UsuarioId", userId);
        cmd.Parameters.AddWithValue("@EventoId", req.EventoId);

        conn.Open();
        cmd.ExecuteNonQuery();

        return Ok(new { Message = "Inscripción cancelada" });
    }


}

public record RegisterEventRequest(int EventoId);


