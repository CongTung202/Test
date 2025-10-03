using System;
using System.Collections.Generic;

namespace NemeShop.Models;

public partial class LoaiSanPham
{
    public int MaLoai { get; set; }

    public string TenLoai { get; set; } = null!;

    public string? MoTa { get; set; }

    public bool TrangThai { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
