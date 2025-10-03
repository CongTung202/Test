using System;
using System.Collections.Generic;

namespace NemeShop.Models;

public partial class KhachHang
{
    public int MaKh { get; set; }

    public string HoTen { get; set; } = null!;

    public string? Email { get; set; }

    public string MatKhau { get; set; } = null!;

    public string? DienThoai { get; set; }

    public string? DiaChi { get; set; }

    public bool TrangThai { get; set; }

    public virtual ICollection<DanhGiaSanPham> DanhGiaSanPhams { get; set; } = new List<DanhGiaSanPham>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}
