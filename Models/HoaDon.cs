using System;
using System.Collections.Generic;

namespace NemeShop.Models;

public partial class HoaDon
{
    public int MaHd { get; set; }

    public DateTime NgayLap { get; set; }

    public decimal TongTien { get; set; }

    public decimal? TienGiamGia { get; set; }

    public decimal ThanhTien { get; set; }

    public int MaKh { get; set; }

    public string? MaVoucher { get; set; }

    public string? DiaChiGiaoHang { get; set; }

    public string? SdtgiaoHang { get; set; }

    public string? GhiChu { get; set; }

    public int TrangThai { get; set; }

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    public virtual KhachHang MaKhNavigation { get; set; } = null!;

    public virtual MaGiamGium? MaVoucherNavigation { get; set; }
}
