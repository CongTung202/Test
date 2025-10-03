using System;
using System.Collections.Generic;

namespace NemeShop.Models;

public partial class DanhGiaSanPham
{
    public int MaDanhGia { get; set; }

    public int MaKh { get; set; }

    public int MaSp { get; set; }

    public int SoSao { get; set; }

    public string? NoiDung { get; set; }

    public bool LaYeuThich { get; set; }

    public DateTime NgayTao { get; set; }

    public bool TrangThai { get; set; }

    public virtual KhachHang MaKhNavigation { get; set; } = null!;

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
