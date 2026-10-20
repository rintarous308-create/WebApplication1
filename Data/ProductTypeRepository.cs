using Microsoft.Data.SqlClient;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class ProductTypeRepository
    {
        private readonly string _connectionString;

        public ProductTypeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("ไม่พบ Connection String 'DefaultConnection'");
        }

        // ดึงประเภทสินค้าทั้งหมด สำหรับใส่ใน Dropdown
        public List<ProductType> GetAll()
        {
            List<ProductType> types = new List<ProductType>();

            string sql = "SELECT id_pt, name_pt FROM product_type ORDER BY name_pt";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        types.Add(new ProductType
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id_pt")),
                            Name = reader.IsDBNull(reader.GetOrdinal("name_pt")) ? null : reader.GetString(reader.GetOrdinal("name_pt"))
                        });
                    }
                }
            }

            return types;
        }
    }
}