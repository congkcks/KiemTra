using System;
using System.Collections.Generic;

namespace KiemTra.Models;

public partial class ThaoTacSua
{
    public int MaThaoTac { get; set; }

    public string username { get; set; } = null!;

    public string LoaiThaoTac { get; set; } = null!;

    public DateTime NgayThaoTac { get; set; }

    public string? ChiTietThaoTac { get; set; }

    //public virtual NhanVien MaNhanVienNavigation { get; set; } = null!;
}
