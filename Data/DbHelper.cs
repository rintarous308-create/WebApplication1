using Microsoft.Data.SqlClient;

namespace WebApplication1.Data
{
    public class DbHelper
    {
        private readonly string _connectionString;


        public DbHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ProductDB")!;
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
