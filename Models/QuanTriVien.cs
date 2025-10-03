using System;
using System.Collections.Generic;

namespace NemeShop.Models;

public partial class QuanTriVien
{
    public int MaQtv { get; set; }

    public string HoTen { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string? DienThoai { get; set; }

    public string? DiaChi { get; set; }

    public bool TrangThai { get; set; }
}
