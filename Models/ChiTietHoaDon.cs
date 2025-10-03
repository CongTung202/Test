using System;
using System.Collections.Generic;

namespace NemeShop.Models;

public partial class ChiTietHoaDon
{
    public int MaCthd { get; set; }

    public int MaHd { get; set; }

    public int MaSp { get; set; }

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public decimal ThanhTien { get; set; }

    public virtual HoaDon MaHdNavigation { get; set; } = null!;

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
