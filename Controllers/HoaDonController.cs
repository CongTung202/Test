using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NemeShop.Models;
using System;
using System.Threading.Tasks;

namespace NemeShop.Controllers
{
    public class HoaDonController : Controller
    {
        private readonly QuanLyTapHoaContext _context;

        public HoaDonController(QuanLyTapHoaContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // để khớp với token trong form
        public async Task<IActionResult> MuaNgay(int maSp, int soLuong)
        {
            // Kiểm tra đăng nhập
            var maKh = HttpContext.Session.GetInt32("MaKh"); // ✅ khớp với SetInt32 bên AccountController
            if (maKh == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để mua ngay." });
            }

            var sp = await _context.SanPhams.FirstOrDefaultAsync(x => x.MaSp == maSp);
            if (sp == null)
                return Json(new { success = false, message = "Không tìm thấy sản phẩm." });

            if (sp.SoLuong < soLuong)
                return Json(new { success = false, message = "Sản phẩm không đủ số lượng trong kho." });

            // Tạo hóa đơn
            var hoaDon = new HoaDon
            {
                MaKh = maKh.Value,
                NgayLap = DateTime.Now,
                TongTien = sp.DonGia * soLuong,
                ThanhTien = sp.DonGia * soLuong,
                DiaChiGiaoHang = "Đang cập nhật",
                SdtgiaoHang = "Đang cập nhật",
                GhiChu = "Mua ngay"
            };

            _context.HoaDons.Add(hoaDon);
            await _context.SaveChangesAsync();

            // Tạo chi tiết hóa đơn
            var cthd = new ChiTietHoaDon
            {
                MaHd = hoaDon.MaHd,
                MaSp = sp.MaSp,
                SoLuong = soLuong,
                DonGia = sp.DonGia,
                ThanhTien = sp.DonGia * soLuong
            };
            _context.ChiTietHoaDons.Add(cthd);

            // Trừ kho
            sp.SoLuong -= soLuong;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Đặt hàng thành công!",
                redirectUrl = Url.Action("Details", "HoaDon", new { id = hoaDon.MaHd })
            });
        }

        // Trang chi tiết hóa đơn
        public async Task<IActionResult> Details(int id)
        {
            var hd = await _context.HoaDons
                .Include(h => h.ChiTietHoaDons)
                .ThenInclude(ct => ct.MaSpNavigation)
                .FirstOrDefaultAsync(h => h.MaHd == id);

            if (hd == null) return NotFound();

            return View(hd);
        }
    }
}
