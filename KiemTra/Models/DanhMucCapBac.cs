using System;
using System.Collections.Generic;

namespace KiemTra.Models;

public partial class DanhMucCapBac
{
    public int MaCapBac { get; set; }

    public string TenCapBac { get; set; } = null!;

    public string? MoTaCapBac { get; set; }
}
