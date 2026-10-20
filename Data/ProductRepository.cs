using Microsoft.Data.SqlClient;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class ProductRepository
    {
        private readonly string _connectionString;

        // สมมติว่า connection string ชื่อ "DefaultConnection" อยู่ใน appsettings.json
        // ถ้าตั้งชื่ออื่น ให้แก้ตรงนี้
        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("ไม่พบ Connection String 'DefaultConnection'");
        }

        // ค้นหาสินค้าทั้งหมด หรือค้นหาตาม keyword (ชื่อสินค้า)
        public List<Product> Search(string? keyword)
        {
            List<Product> products = new List<Product>();

            string sql = @"
                SELECT p.id_pd, p.name_pd, p.price_pd, p.detail_pd, p.id_pt, t.name_pt
                FROM products p
                INNER JOIN product_type t ON p.id_pt = t.id_pt
                WHERE (@keyword IS NULL OR p.name_pd LIKE '%' + @keyword + '%')
                ORDER BY p.id_pd DESC";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@keyword", (object?)keyword ?? DBNull.Value);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(MapProduct(reader));
                    }
                }
            }

            return products;
        }

        // ค้นหาสินค้าตัวเดียวจาก Id
        public Product? SearchById(int id)
        {
            string sql = @"
                SELECT p.id_pd, p.name_pd, p.price_pd, p.detail_pd, p.id_pt, t.name_pt
                FROM products p
                INNER JOIN product_type t ON p.id_pt = t.id_pt
                WHERE p.id_pd = @id";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapProduct(reader);
                    }
                }
            }

            return null;
        }

        // เพิ่มสินค้าใหม่ คืนค่า Id ที่เพิ่งสร้าง
        public int Insert(Product product)
        {
            string sql = @"
                INSERT INTO products (name_pd, price_pd, detail_pd, id_pt)
                VALUES (@name, @price, @detail, @typeId);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@name", product.Name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@price", product.Price);
                cmd.Parameters.AddWithValue("@detail", product.Detail ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@typeId", product.TypeId);

                conn.Open();
                object? result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        // แก้ไขสินค้า คืนค่าจำนวนแถวที่ได้รับผลกระทบ
        public int Update(Product product)
        {
            string sql = @"
                UPDATE products
                SET name_pd = @name,
                    price_pd = @price,
                    detail_pd = @detail,
                    id_pt = @typeId
                WHERE id_pd = @id";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@name", product.Name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@price", product.Price);
                cmd.Parameters.AddWithValue("@detail", product.Detail ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@typeId", product.TypeId);
                cmd.Parameters.AddWithValue("@id", product.Id);

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        // ลบสินค้าตาม Id
        public int Delete(int id)
        {
            string sql = "DELETE FROM products WHERE id_pd = @id";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        // ช่วย map ข้อมูลจาก reader ไปเป็น Product object
        private static Product MapProduct(SqlDataReader reader)
        {
            return new Product
            {
                Id = reader.GetInt32(reader.GetOrdinal("id_pd")),
                Name = reader.IsDBNull(reader.GetOrdinal("name_pd")) ? null : reader.GetString(reader.GetOrdinal("name_pd")),
                Price = reader.GetDouble(reader.GetOrdinal("price_pd")),
                Detail = reader.IsDBNull(reader.GetOrdinal("detail_pd")) ? null : reader.GetString(reader.GetOrdinal("detail_pd")),
                TypeId = reader.GetInt32(reader.GetOrdinal("id_pt")),
                TypeName = reader.IsDBNull(reader.GetOrdinal("name_pt")) ? null : reader.GetString(reader.GetOrdinal("name_pt"))
            };
        }
    }
}