using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace EventosApp.Data
{
    public class DbHelper
    {
        private readonly string _conn;
        public DbHelper(IConfiguration config)
        {
            _conn = config.GetConnectionString("DefaultConnection");
        }
        public SqlConnection GetConnection() => new SqlConnection(_conn);
    }
}
