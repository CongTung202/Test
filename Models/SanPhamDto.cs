namespace NemeShop.Models
{
    public class SanPhamDto
    {
        public int MaSp { get; set; }
        public string TenSp { get; set; }
        public string MoTa { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public string HinhAnh { get; set; }  // giữ đường dẫn ảnh cũ
        public int? MaLoai { get; set; }
        public decimal PhanTramGiam { get; set; }
        public bool TrangThai { get; set; }
    }

}
