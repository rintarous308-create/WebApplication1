using Microsoft.Data.SqlClient;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class MemberRepository
    {
        private readonly string _connectionString;

        public MemberRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("ไม่พบ Connection String 'DefaultConnection'");
        }

        // ค้นหาสมาชิกทั้งหมด หรือค้นหาตาม keyword (ชื่อ หรือ อีเมล)
        public List<Member> Search(string? keyword)
        {
            List<Member> members = new List<Member>();

            string sql = @"
                SELECT id_mem, name_mem, email_mem, password_mem, gender_mem,
                       birthday_mem, phone_mem, address_mem, country_mem
                FROM members
                WHERE (@keyword IS NULL
                       OR name_mem LIKE '%' + @keyword + '%'
                       OR email_mem LIKE '%' + @keyword + '%')
                ORDER BY id_mem DESC";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@keyword", (object?)keyword ?? DBNull.Value);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        members.Add(MapMember(reader));
                    }
                }
            }

            return members;
        }

        // ค้นหาสมาชิกตัวเดียวจาก Id
        public Member? SearchById(int id)
        {
            string sql = @"
                SELECT id_mem, name_mem, email_mem, password_mem, gender_mem,
                       birthday_mem, phone_mem, address_mem, country_mem
                FROM members
                WHERE id_mem = @id";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapMember(reader);
                    }
                }
            }

            return null;
        }

        // เพิ่มสมาชิกใหม่ คืนค่า Id ที่เพิ่งสร้าง
        public int Insert(Member member)
        {
            string sql = @"
                INSERT INTO members
                    (name_mem, email_mem, password_mem, gender_mem, birthday_mem, phone_mem, address_mem, country_mem)
                VALUES
                    (@name, @email, @password, @gender, @birthday, @phone, @address, @country);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                AddMemberParameters(cmd, member);
                conn.Open();
                object? result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        // แก้ไขข้อมูลสมาชิก คืนค่าจำนวนแถวที่ได้รับผลกระทบ
        public int Update(Member member)
        {
            string sql = @"
                UPDATE members
                SET name_mem = @name,
                    email_mem = @email,
                    password_mem = @password,
                    gender_mem = @gender,
                    birthday_mem = @birthday,
                    phone_mem = @phone,
                    address_mem = @address,
                    country_mem = @country
                WHERE id_mem = @id";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                AddMemberParameters(cmd, member);
                cmd.Parameters.AddWithValue("@id", member.Id);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        // ลบสมาชิกตาม Id
        public int Delete(int id)
        {
            string sql = "DELETE FROM members WHERE id_mem = @id";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        private static void AddMemberParameters(SqlCommand cmd, Member member)
        {
            cmd.Parameters.AddWithValue("@name", member.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@email", member.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@password", member.Password ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@gender", member.Gender ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@birthday", (object?)member.Birthday ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@phone", member.Phone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@address", member.Address ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@country", member.Country ?? (object)DBNull.Value);
        }

        // ช่วย map ข้อมูลจาก reader ไปเป็น Member object
        private static Member MapMember(SqlDataReader reader)
        {
            return new Member
            {
                Id = reader.GetInt32(reader.GetOrdinal("id_mem")),
                Name = reader.IsDBNull(reader.GetOrdinal("name_mem")) ? null : reader.GetString(reader.GetOrdinal("name_mem")),
                Email = reader.IsDBNull(reader.GetOrdinal("email_mem")) ? null : reader.GetString(reader.GetOrdinal("email_mem")),
                Password = reader.IsDBNull(reader.GetOrdinal("password_mem")) ? null : reader.GetString(reader.GetOrdinal("password_mem")),
                Gender = reader.IsDBNull(reader.GetOrdinal("gender_mem")) ? null : reader.GetString(reader.GetOrdinal("gender_mem")),
                Birthday = reader.IsDBNull(reader.GetOrdinal("birthday_mem")) ? null : reader.GetDateTime(reader.GetOrdinal("birthday_mem")),
                Phone = reader.IsDBNull(reader.GetOrdinal("phone_mem")) ? null : reader.GetString(reader.GetOrdinal("phone_mem")),
                Address = reader.IsDBNull(reader.GetOrdinal("address_mem")) ? null : reader.GetString(reader.GetOrdinal("address_mem")),
                Country = reader.IsDBNull(reader.GetOrdinal("country_mem")) ? null : reader.GetString(reader.GetOrdinal("country_mem"))
            };
        }
    }
}