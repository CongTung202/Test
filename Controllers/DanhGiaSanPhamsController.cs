using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NemeShop.Filters;
using NemeShop.Hubs;
using NemeShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NemeShop.Controllers
{
    [AuthorizeAdmin]
    public class DanhGiaSanPhamsController : Controller
    {
        private readonly QuanLyTapHoaContext _context;
        private readonly IHubContext<RealtimeHub> _hub;

        public DanhGiaSanPhamsController(QuanLyTapHoaContext context, IHubContext<RealtimeHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        // GET: DanhGiaSanPhams
        public async Task<IActionResult> Index()
        {
            var quanLyTapHoaContext = _context.DanhGiaSanPhams.Include(d => d.MaKhNavigation).Include(d => d.MaSpNavigation);
            return View(await quanLyTapHoaContext.ToListAsync());
        }

        // GET: DanhGiaSanPhams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhGiaSanPham = await _context.DanhGiaSanPhams
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaSpNavigation)
                .FirstOrDefaultAsync(m => m.MaDanhGia == id);
            if (danhGiaSanPham == null)
            {
                return NotFound();
            }

            return View(danhGiaSanPham);
        }

        // GET: Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "HoTen");
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "TenSp");
            return View();
        }

        // POST: Create (AJAX)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DanhGiaSanPhamDto dto)
        {
            if (dto == null) return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = string.Join(", ", errors) });
            }

            var entity = new DanhGiaSanPham
            {
                MaKh = dto.MaKh ?? 0,
                MaSp = dto.MaSp ?? 0,
                SoSao = dto.SoSao ?? 5,
                NoiDung = dto.NoiDung,
                LaYeuThich = dto.LaYeuThich,
                NgayTao = DateTime.Now,
                TrangThai = dto.TrangThai
            };

            _context.DanhGiaSanPhams.Add(entity);
            await _context.SaveChangesAsync();
            await _hub.Clients.All.SendAsync("ReviewAdded", entity.MaSp, entity.MaDanhGia);

            return Json(new { success = true, message = "Thêm đánh giá thành công" });
        }
        // POST: DanhGiaSanPhams/CreateForm (multipart/form-data but without file)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateForm()
        {
            try
            {
                var form = Request.Form;
                if (form == null) return Json(new { success = false, message = "Dữ liệu không hợp lệ" });

                int maSp = int.Parse(form["MaSp"]);
                int soSao = int.Parse(form["SoSao"]);
                string noiDung = form["NoiDung"];
                bool laYeuThich = form["LaYeuThich"].Count > 0 && form["LaYeuThich"] == "true";
                int maKh = 0;
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("MaKh")))
                    int.TryParse(HttpContext.Session.GetString("MaKh"), out maKh);

                var entity = new DanhGiaSanPham
                {
                    MaKh = maKh,
                    MaSp = maSp,
                    SoSao = soSao,
                    NoiDung = noiDung,
                    LaYeuThich = laYeuThich,
                    NgayTao = DateTime.Now,
                    TrangThai = false
                };

                _context.DanhGiaSanPhams.Add(entity);
                await _context.SaveChangesAsync();

                // gửi event realtime để client cập nhật (nếu dùng)
                var hub = HttpContext.RequestServices.GetService<Microsoft.AspNetCore.SignalR.IHubContext<NemeShop.Hubs.RealtimeHub>>();
                if (hub != null)
                {
                    await hub.Clients.All.SendAsync("ReviewAdded", maSp, entity.MaDanhGia);
                }

                return Json(new { success = true, message = "Gửi đánh giá thành công. Đang chờ duyệt." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: DanhGiaSanPhams/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var danhGia = await _context.DanhGiaSanPhams.FindAsync(id);
            if (danhGia == null) return NotFound();

            var dto = new DanhGiaUpdateTrangThaiDto
            {
                MaDanhGia = danhGia.MaDanhGia,
                TrangThai = danhGia.TrangThai
            };

            return View(dto);
        }

        // POST: DanhGiaSanPhams/Edit
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] DanhGiaUpdateTrangThaiDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage);
                return Json(new { success = false, message = string.Join("; ", errors) });
            }

            var entity = await _context.DanhGiaSanPhams.FindAsync(dto.MaDanhGia);
            if (entity == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đánh giá" });
            }

            entity.TrangThai = dto.TrangThai;

            try
            {
                _context.Update(entity);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Cập nhật trạng thái thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        // GET: DanhGiaSanPhams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhGiaSanPham = await _context.DanhGiaSanPhams
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaSpNavigation)
                .FirstOrDefaultAsync(m => m.MaDanhGia == id);
            if (danhGiaSanPham == null)
            {
                return NotFound();
            }

            return View(danhGiaSanPham);
        }

        // POST: DanhGiaSanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var danhGiaSanPham = await _context.DanhGiaSanPhams.FindAsync(id);
            if (danhGiaSanPham != null)
            {
                _context.DanhGiaSanPhams.Remove(danhGiaSanPham);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DanhGiaSanPhamExists(int id)
        {
            return _context.DanhGiaSanPhams.Any(e => e.MaDanhGia == id);
        }
    }
}
