using System;
using System.Collections.Generic;

namespace NemeShop.Models;

public partial class SanPham
{
    public int MaSp { get; set; }

    public string TenSp { get; set; } = null!;

    public string? MoTa { get; set; }

    public decimal DonGia { get; set; }

    public int SoLuong { get; set; }

    public string? HinhAnh { get; set; }

    public int? MaLoai { get; set; }

    public decimal PhanTramGiam { get; set; }

    public bool TrangThai { get; set; }

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    public virtual ICollection<DanhGiaSanPham> DanhGiaSanPhams { get; set; } = new List<DanhGiaSanPham>();

    public virtual LoaiSanPham? MaLoaiNavigation { get; set; }
}
