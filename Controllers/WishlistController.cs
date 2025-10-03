using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NemeShop.Models;

namespace NemeShop.Controllers
{
    public class WishlistController : Controller
    {
        private readonly QuanLyTapHoaContext _context;

        public WishlistController(QuanLyTapHoaContext context)
        {
            _context = context;
        }

        // 📌 Trang "Yêu thích"
        public async Task<IActionResult> MyWishlist()
        {
            var maKh = HttpContext.Session.GetInt32("MaKh");
            if (maKh == null)
                return RedirectToAction("Login", "Account");

            var wishlist = await _context.DanhGiaSanPhams
                .Where(x => x.MaKh == maKh && x.LaYeuThich)
                .OrderByDescending(x => x.NgayTao)
                .Select(x => new DanhGiaSanPhamDto
                {
                    MaDanhGia = x.MaDanhGia,
                    MaKh = x.MaKh,
                    MaSp = x.MaSp,
                    SoSao = x.SoSao,
                    NoiDung = x.NoiDung,
                    LaYeuThich = x.LaYeuThich,
                    NgayTao = x.NgayTao,
                    TrangThai = x.TrangThai,
                    TenSanPham = x.MaSpNavigation.TenSp,
                    TenKhachHang = x.MaKhNavigation.HoTen
                })
                .ToListAsync();

            return View("MyWishlist", wishlist);
        }

        // 📌 Thêm vào Wishlist
        [HttpPost]
        public async Task<IActionResult> AddToWishlist([FromBody] DanhGiaSanPhamDto dto)
        {
            var maKh = HttpContext.Session.GetInt32("MaKh");
            if (maKh == null)
                return Unauthorized(new { success = false, message = "Bạn cần đăng nhập" });

            var item = await _context.DanhGiaSanPhams
                .FirstOrDefaultAsync(x => x.MaKh == maKh && x.MaSp == dto.MaSp);

            if (item != null)
            {
                if (item.LaYeuThich)
                {
                    return Json(new { success = true, message = "Sản phẩm đã có trong yêu thích" });
                }

                item.LaYeuThich = true;
                item.NgayTao = DateTime.Now;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã thêm lại vào yêu thích" });
            }

            var entity = new DanhGiaSanPham
            {
                MaKh = maKh.Value,
                MaSp = dto.MaSp ?? 0,
                SoSao = 5,
                NoiDung = "Đã thêm vào yêu thích",
                LaYeuThich = true,
                NgayTao = DateTime.Now,
                TrangThai = true
            };

            _context.DanhGiaSanPhams.Add(entity);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã thêm vào danh sách yêu thích!" });
        }

        // 📌 Xóa khỏi Wishlist
        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlist([FromBody] DanhGiaSanPhamDto dto)
        {
            var maKh = HttpContext.Session.GetInt32("MaKh");
            if (maKh == null)
                return Unauthorized(new { success = false, message = "Bạn cần đăng nhập" });

            var item = await _context.DanhGiaSanPhams
                .FirstOrDefaultAsync(x => x.MaKh == maKh && x.MaSp == dto.MaSp && x.LaYeuThich);

            if (item == null)
                return NotFound(new { success = false, message = "Không tìm thấy trong yêu thích" });

            item.LaYeuThich = false;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã xóa khỏi danh sách yêu thích" });
        }
    }
}
