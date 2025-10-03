using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NemeShop.Models;
using System.Linq;
using System.Threading.Tasks;

namespace NemeShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly QuanLyTapHoaContext _context;
        public HomeController(QuanLyTapHoaContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var products = await _context.SanPhams
                .Include(p => p.MaLoaiNavigation)
                .Where(p => p.TrangThai)
                .OrderByDescending(p => p.MaSp)
                .Take(24)
                .ToListAsync();

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.SanPhams
                .Include(p => p.MaLoaiNavigation)
                .FirstOrDefaultAsync(p => p.MaSp == id && p.TrangThai);

            if (product == null) return NotFound();

            // Lấy đánh giá kèm tên khách hàng
            var reviews = await _context.DanhGiaSanPhams
                .Where(r => r.MaSp == id && r.TrangThai)
                .OrderByDescending(r => r.NgayTao)
                .Select(r => new DanhGiaSanPhamDto
                {
                    MaDanhGia = r.MaDanhGia,
                    MaKh = r.MaKh,
                    MaSp = r.MaSp,
                    SoSao = r.SoSao,
                    NoiDung = r.NoiDung,
                    LaYeuThich = r.LaYeuThich,
                    NgayTao = r.NgayTao,
                    TrangThai = r.TrangThai,
                    TenKhachHang = r.MaKhNavigation.HoTen, // giả sử có cột HoTen
                    TenSanPham = r.MaSpNavigation.TenSp
                })
                .ToListAsync();

            ViewBag.Reviews = reviews;
            return View(product);
        }
        [HttpGet]
        public async Task<IActionResult> AddReview(int spId)
        {
            // Lấy sản phẩm để hiển thị tên
            var product = await _context.SanPhams.FindAsync(spId);
            if (product == null) return NotFound();

            var dto = new DanhGiaSanPhamDto
            {
                MaSp = spId,
                TenSanPham = product.TenSp,
                NgayTao = DateTime.Now,
                TrangThai = true
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(DanhGiaSanPhamDto dto)
        {
            if (!ModelState.IsValid)
            {
                // Trả về partial lỗi nếu dữ liệu không hợp lệ
                var reviewsError = await _context.DanhGiaSanPhams
                    .Where(r => r.MaSp == dto.MaSp && r.TrangThai)
                    .Select(r => new DanhGiaSanPhamDto
                    {
                        MaDanhGia = r.MaDanhGia,
                        MaKh = r.MaKh,
                        MaSp = r.MaSp,
                        SoSao = r.SoSao,
                        NoiDung = r.NoiDung,
                        LaYeuThich = r.LaYeuThich,
                        NgayTao = r.NgayTao,
                        TrangThai = r.TrangThai,
                        TenKhachHang = r.MaKhNavigation.HoTen,
                        TenSanPham = r.MaSpNavigation.TenSp
                    })
                    .ToListAsync();

                return PartialView("_ReviewListPartial", reviewsError);
            }

            var entity = new DanhGiaSanPham
            {
                MaKh = dto.MaKh ?? 1003, // fallback
                MaSp = dto.MaSp ?? 1,
                SoSao = dto.SoSao ?? 5,
                NoiDung = dto.NoiDung,
                LaYeuThich = dto.LaYeuThich,
                NgayTao = DateTime.Now,
                TrangThai = true
            };

            _context.DanhGiaSanPhams.Add(entity);
            await _context.SaveChangesAsync();

            // load lại danh sách đánh giá
            var reviews = await _context.DanhGiaSanPhams
                .Where(r => r.MaSp == entity.MaSp && r.TrangThai)
                .Select(r => new DanhGiaSanPhamDto
                {
                    MaDanhGia = r.MaDanhGia,
                    MaKh = r.MaKh,
                    MaSp = r.MaSp,
                    SoSao = r.SoSao,
                    NoiDung = r.NoiDung,
                    LaYeuThich = r.LaYeuThich,
                    NgayTao = r.NgayTao,
                    TrangThai = r.TrangThai,
                    TenKhachHang = r.MaKhNavigation.HoTen,
                    TenSanPham = r.MaSpNavigation.TenSp
                })
                .ToListAsync();

            return PartialView("_ReviewListPartial", reviews);
        }


        [HttpGet]
        public async Task<IActionResult> SearchProducts(string q, int? cat, decimal? min, decimal? max, string sort = "popular", int page = 1, int pageSize = 24)
        {
            var query = _context.SanPhams.Include(p => p.MaLoaiNavigation).AsQueryable();
            query = query.Where(p => p.TrangThai);

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(p => EF.Functions.Like(p.TenSp, $"%{q}%"));

            if (cat.HasValue) query = query.Where(p => p.MaLoai == cat.Value);
            if (min.HasValue) query = query.Where(p => p.DonGia >= min.Value);
            if (max.HasValue && max.Value > 0) query = query.Where(p => p.DonGia <= max.Value);

            query = sort switch
            {
                "priceAsc" => query.OrderBy(p => p.DonGia),
                "priceDesc" => query.OrderByDescending(p => p.DonGia),
                "newest" => query.OrderByDescending(p => p.MaSp),
                _ => query.OrderByDescending(p => p.PhanTramGiam)
            };

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return PartialView("_ProductGridPartial", items);
        }
    }
}
