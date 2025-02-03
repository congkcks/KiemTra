using System;
using System.Collections.Generic;

namespace KiemTra.Models;

public partial class QuaTrinhCongTac
{
    public int MaQuaTrinh { get; set; }

    public int MaNhanVien { get; set; }

    public DateOnly NgayBatDau { get; set; }

    public DateOnly? NgayKetThuc { get; set; }

    public string? ChucVu { get; set; }

    public string? PhongBan { get; set; }

    public virtual NhanVien MaNhanVienNavigation { get; set; } = null!;
}
