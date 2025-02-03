using System;
using System.Collections.Generic;

namespace KiemTra.Models;

public partial class NhanVien
{
    public int MaNhanVien { get; set; }

    public string HoTen { get; set; } = null!;

    public string? DiaChi { get; set; }

    public string? QueQuan { get; set; }

    public DateOnly NgayBatDau { get; set; }

    public DateOnly? NgayKetThuc { get; set; }

    public string? ChucVu { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<QuaTrinhCongTac> QuaTrinhCongTacs { get; set; } = new List<QuaTrinhCongTac>();

    //public virtual ICollection<ThaoTacSua> ThaoTacSuas { get; set; } = new List<ThaoTacSua>();

    public virtual ICollection<VanBang> VanBangs { get; set; } = new List<VanBang>();
}
