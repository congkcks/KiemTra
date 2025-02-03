using System.ComponentModel.DataAnnotations;

namespace KiemTra.Models
{
    public class User
    {
        [Required]
        [StringLength(100)]
        public string? Username { get; set; }  // Tên người dùng

        [Required]
        [StringLength(100)]
        public string? Password { get; set; }  // Mật khẩu

        public string Role { get; set; } = "User"; // Vai trò mặc định là "User"
    }
}
