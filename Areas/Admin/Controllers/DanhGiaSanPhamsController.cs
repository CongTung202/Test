using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NemeShop.Filters;
using NemeShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NemeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdmin]
    public class DanhGiaSanPhamsController : Controller
    {
        private readonly QuanLyTapHoaContext _context;

        public DanhGiaSanPhamsController(QuanLyTapHoaContext context)
        {
            _context = context;
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
            if (dto == null)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return Json(new { success = false, message = string.Join(", ", errors) });
            }

            var entity = new DanhGiaSanPham
            {
                MaKh = dto.MaKh.Value,
                MaSp = dto.MaSp.Value,
                SoSao = dto.SoSao.Value,
                NoiDung = dto.NoiDung,
                LaYeuThich = dto.LaYeuThich,
                NgayTao = DateTime.Now,
                TrangThai = dto.TrangThai
            };

            _context.DanhGiaSanPhams.Add(entity);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Thêm đánh giá thành công" });
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