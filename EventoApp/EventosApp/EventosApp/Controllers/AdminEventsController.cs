using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using EventosApp.Data;

[Authorize]
[ApiController]
[Route("api/admin/events")]
public class AdminEventsController : ControllerBase
{
    private readonly DbHelper _db;

    public AdminEventsController(DbHelper db)
    {
        _db = db;
    }

    private int UserId =>
        int.Parse(User.Claims.First(c => c.Type == "UserId").Value);

    [HttpGet]
    public IActionResult List()
    {
        var list = new List<object>();

        using var conn = _db.GetConnection();
        using var cmd = new SqlCommand("usp_Admin_ListMyEvents", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@UserId", UserId);

        conn.Open();
        using var r = cmd.ExecuteReader();

        while (r.Read())
        {
            list.Add(new
            {
                id = r["Id"],
                title = r["Titulo"],
                description = r["Descripcion"],
                eventType = r["Tipo"],
                localizacion = r["Localizacion"],
                startDate = r["Inicio"],
                finalDate = r["Final"],
                inscritos = r["Inscritos"]
            });

        }

        return Ok(list);
    }

    [HttpPost]
    public IActionResult Create(CreateEventRequest e)
    {
        using var conn = _db.GetConnection();
        using var cmd = new SqlCommand("usp_CreateEvent", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@Titulo", e.Titulo);
        cmd.Parameters.AddWithValue("@Descripcion", e.Descripcion);
        cmd.Parameters.AddWithValue("@Tipo", e.Tipo);
        cmd.Parameters.AddWithValue("@Localizacion", e.Localizacion);
        cmd.Parameters.AddWithValue("@Inicio", e.Inicio);
        cmd.Parameters.AddWithValue("@Final", e.Final);
        cmd.Parameters.AddWithValue("@CreadoPorId", UserId);

        conn.Open();
        cmd.ExecuteNonQuery();

        return Ok();
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, CreateEventRequest e)
    {
        using var conn = _db.GetConnection();
        using var cmd = new SqlCommand("usp_Admin_UpdateEvent", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Titulo", e.Titulo);
        cmd.Parameters.AddWithValue("@Descripcion", e.Descripcion);
        cmd.Parameters.AddWithValue("@Tipo", e.Tipo);
        cmd.Parameters.AddWithValue("@Localizacion", e.Localizacion);
        cmd.Parameters.AddWithValue("@Inicio", e.Inicio);
        cmd.Parameters.AddWithValue("@Final", e.Final);
        cmd.Parameters.AddWithValue("@UserId", UserId);

        conn.Open();
        int rows = cmd.ExecuteNonQuery();

        if (rows == 0)
            return Forbid("No puedes editar este evento");

        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        using var conn = _db.GetConnection();
        using var cmd = new SqlCommand("usp_Admin_DeleteEvent", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@UserId", UserId);

        conn.Open();
        int rows = cmd.ExecuteNonQuery();

        if (rows == 0)
            return Forbid("No puedes eliminar este evento");

        return Ok();
    }
}

public record CreateEventRequest(
    string Titulo,
    string Descripcion,
    string Tipo,
    string Localizacion,
    DateTime Inicio,
    DateTime? Final
);


