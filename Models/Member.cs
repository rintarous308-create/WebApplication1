using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Member
    {
        public int Id { get; set; }              // id_mem

        [Display(Name = "ชื่อ")]
        public string? Name { get; set; }        // name_mem

        [Display(Name = "อีเมล")]
        public string? Email { get; set; }       // email_mem

        [Display(Name = "รหัสผ่าน")]
        public string? Password { get; set; }    // password_mem

        [Display(Name = "เพศ")]
        public string? Gender { get; set; }      // gender_mem

        [Display(Name = "วันเกิด")]
        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }  // birthday_mem

        [Display(Name = "เบอร์โทร")]
        public string? Phone { get; set; }       // phone_mem

        [Display(Name = "ที่อยู่")]
        public string? Address { get; set; }     // address_mem

        [Display(Name = "ประเทศ")]
        public string? Country { get; set; }     // country_mem
    }
}