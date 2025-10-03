using System;
using System.Collections.Generic;

namespace NemeShop.Models;

public partial class MaGiamGium
{
    public string MaVoucher { get; set; } = null!;

    public string TenVoucher { get; set; } = null!;

    public string? MoTa { get; set; }

    public decimal? PhanTramGiam { get; set; }

    public decimal? GiamTienTrucTiep { get; set; }

    public decimal? GiaTriDonToiThieu { get; set; }

    public int SoLuong { get; set; }

    public int DaSuDung { get; set; }

    public DateTime NgayBatDau { get; set; }

    public DateTime NgayKetThuc { get; set; }

    public bool TrangThai { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}
