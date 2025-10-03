using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NemeShop.Filters;
using NemeShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace NemeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdmin]
    public class SanPhamsController : Controller
    {
        private readonly QuanLyTapHoaContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly string _imagesFolder;

        public SanPhamsController(QuanLyTapHoaContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
            _imagesFolder = Path.Combine(_environment.WebRootPath, "images");

            // Ensure images directory exists
            if (!Directory.Exists(_imagesFolder))
            {
                Directory.CreateDirectory(_imagesFolder);
            }
        }

        // GET: SanPhams
        public async Task<IActionResult> Index()
        {
            var quanLyTapHoaContext = _context.SanPhams.Include(s => s.MaLoaiNavigation);
            return View(await quanLyTapHoaContext.ToListAsync());
        }

        // GET: SanPhams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaLoaiNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // GET: SanPhams/Create
        public IActionResult Create()
        {
            ViewData["MaLoai"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai");
            return View();
        }

        // POST: SanPhams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPham sanPham, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string fileName = null;
                if (file != null && file.Length > 0)
                {
                    // Validate file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    var fileExtension = Path.GetExtension(file.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("", "Chỉ chấp nhận file ảnh: JPG, JPEG, PNG, GIF, BMP");
                        ViewData["MaLoai"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai", sanPham.MaLoai);
                        return View(sanPham);
                    }

                    fileName = Guid.NewGuid().ToString() + fileExtension;
                    string filePath = Path.Combine(_imagesFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    // SỬA Ở ĐÂY: Thêm "/images/" vào đường dẫn
                    sanPham.HinhAnh = "/images/" + fileName;
                }

                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaLoai"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai", sanPham.MaLoai);
            return View(sanPham);
        }

        // GET: SanPhams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }

            ViewData["MaLoai"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai", sanPham.MaLoai);
            return View(sanPham);
        }

        // POST: SanPhams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SanPham sanPham, IFormFile? file)
        {
            if (id != sanPham.MaSp)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var entity = await _context.SanPhams.FindAsync(id);
                    if (entity == null)
                    {
                        return NotFound();
                    }

                    if (file != null && file.Length > 0)
                    {
                        // Validate file type
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                        var fileExtension = Path.GetExtension(file.FileName).ToLower();

                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("", "Chỉ chấp nhận file ảnh: JPG, JPEG, PNG, GIF, BMP");
                            ViewData["MaLoai"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai", sanPham.MaLoai);
                            return View(sanPham);
                        }

                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(entity.HinhAnh))
                        {
                            // SỬA Ở ĐÂY: Lấy tên file từ đường dẫn
                            string oldFileName = Path.GetFileName(entity.HinhAnh);
                            if (!string.IsNullOrEmpty(oldFileName))
                            {
                                string oldFilePath = Path.Combine(_imagesFolder, oldFileName);
                                if (System.IO.File.Exists(oldFilePath))
                                {
                                    System.IO.File.Delete(oldFilePath);
                                }
                            }
                        }

                        // Save new image
                        string fileName = Guid.NewGuid().ToString() + fileExtension;
                        string filePath = Path.Combine(_imagesFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        // SỬA Ở ĐÂY: Thêm "/images/" vào đường dẫn
                        entity.HinhAnh = "/images/" + fileName;
                    }

                    // Update other fields
                    entity.TenSp = sanPham.TenSp;
                    entity.MoTa = sanPham.MoTa;
                    entity.DonGia = sanPham.DonGia;
                    entity.SoLuong = sanPham.SoLuong;
                    entity.MaLoai = sanPham.MaLoai;
                    entity.TrangThai = sanPham.TrangThai;
                    entity.PhanTramGiam = sanPham.PhanTramGiam;

                    _context.Update(entity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(sanPham.MaSp))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaLoai"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai", sanPham.MaLoai);
            return View(sanPham);
        }

        // AJAX Edit endpoint
        [HttpPost]
        public async Task<IActionResult> EditAjax(int MaSp, string TenSp, string MoTa, decimal DonGia, int SoLuong, int MaLoai, decimal PhanTramGiam, bool TrangThai, IFormFile file, string HinhAnh)
        {
            try
            {
                var entity = await _context.SanPhams.FindAsync(MaSp);
                if (entity == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại" });
                }

                if (file != null && file.Length > 0)
                {
                    // Validate file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    var fileExtension = Path.GetExtension(file.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return Json(new { success = false, message = "Chỉ chấp nhận file ảnh: JPG, JPEG, PNG, GIF, BMP" });
                    }

                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(entity.HinhAnh))
                    {
                        // SỬA Ở ĐÂY: Lấy tên file từ đường dẫn
                        string oldFileName = Path.GetFileName(entity.HinhAnh);
                        if (!string.IsNullOrEmpty(oldFileName))
                        {
                            string oldFilePath = Path.Combine(_imagesFolder, oldFileName);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
                    }

                    // Save new image
                    string fileName = Guid.NewGuid().ToString() + fileExtension;
                    string filePath = Path.Combine(_imagesFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    // SỬA Ở ĐÂY: Thêm "/images/" vào đường dẫn
                    entity.HinhAnh = "/images/" + fileName;
                }
                else
                {
                    // Keep the old image if no new file is uploaded
                    entity.HinhAnh = HinhAnh;
                }

                // Update other fields
                entity.TenSp = TenSp;
                entity.MoTa = MoTa;
                entity.DonGia = DonGia;
                entity.SoLuong = SoLuong;
                entity.MaLoai = MaLoai;
                entity.TrangThai = TrangThai;
                entity.PhanTramGiam = PhanTramGiam;

                _context.Update(entity);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // GET: SanPhams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaLoaiNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // POST: SanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham != null)
            {
                if (!string.IsNullOrEmpty(sanPham.HinhAnh))
                {
                    // SỬA Ở ĐÂY: Lấy tên file từ đường dẫn
                    string fileName = Path.GetFileName(sanPham.HinhAnh);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        string filePath = Path.Combine(_imagesFolder, fileName);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                }

                _context.SanPhams.Remove(sanPham);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SanPhamExists(int id)
        {
            return _context.SanPhams.Any(e => e.MaSp == id);
        }
    }
}