using System;
using System.Collections.Generic;

namespace KiemTra.Models;

public partial class VanBang
{
    public int MaVanBang { get; set; }

    public int MaNhanVien { get; set; }

    public string TenVanBang { get; set; } = null!;

    public string? NoiCap { get; set; }

    public int NamCap { get; set; }

    public virtual NhanVien MaNhanVienNavigation { get; set; } = null!;
}
