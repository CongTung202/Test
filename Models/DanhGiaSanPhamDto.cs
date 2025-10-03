using System.ComponentModel.DataAnnotations;

namespace NemeShop.Models;

public class DanhGiaSanPhamDto
{
    public int MaDanhGia { get; set; }

    [Required(ErrorMessage = "Khách hàng bắt buộc chọn")]
    public int? MaKh { get; set; }

    [Required(ErrorMessage = "Sản phẩm bắt buộc chọn")]
    public int? MaSp { get; set; }

    [Range(1, 5, ErrorMessage = "Số sao phải từ 1-5")]
    public int? SoSao { get; set; }

    [StringLength(500, ErrorMessage = "Nội dung không quá 500 ký tự")]
    public string? NoiDung { get; set; }

    public bool LaYeuThich { get; set; }

    // Set mặc định khi lưu xuống DB
    public DateTime NgayTao { get; set; } = DateTime.Now;
    public bool TrangThai { get; set; } = true;

    // Dùng để hiển thị
    public string? TenKhachHang { get; set; }
    public string? TenSanPham { get; set; }
}
