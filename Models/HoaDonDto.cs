namespace NemeShop.Models
{
    public class HoaDonDto
    {
        public int? MaHd { get; set; }   // Nullable để phân biệt Create/Edit
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

        // Nếu có chi tiết hóa đơn thì có thêm list
        public List<ChiTietHoaDonsDto>? ChiTietHoaDons { get; set; }
    }

    public class ChiTietHoaDonsDto
    {
        public int MaSp { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
    }
}
