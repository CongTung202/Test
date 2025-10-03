using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NemeShop.Filters;
using NemeShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NemeShop.Controllers
{
    [AuthorizeAdmin]
    public class ChiTietHoaDonsController : Controller
    {
        private readonly QuanLyTapHoaContext _context;

        public ChiTietHoaDonsController(QuanLyTapHoaContext context)
        {
            _context = context;
        }

        // GET: ChiTietHoaDons
        public async Task<IActionResult> Index()
        {
            var quanLyTapHoaContext = _context.ChiTietHoaDons.Include(c => c.MaHdNavigation).Include(c => c.MaSpNavigation);
            return View(await quanLyTapHoaContext.ToListAsync());
        }

        // GET: ChiTietHoaDons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietHoaDon = await _context.ChiTietHoaDons
                .Include(c => c.MaHdNavigation)
                .Include(c => c.MaSpNavigation)
                .FirstOrDefaultAsync(m => m.MaCthd == id);
            if (chiTietHoaDon == null)
            {
                return NotFound();
            }

            return View(chiTietHoaDon);
        }

        // GET: ChiTietHoaDons/Create
        // GET: ChiTietHoaDons/Create
        public IActionResult Create()
        {
            // Dropdown cho Hóa đơn và Sản phẩm
            ViewData["MaHd"] = new SelectList(_context.HoaDons, "MaHd", "MaHd");
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "TenSp");
            return View();
        }

        // POST: ChiTietHoaDons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChiTietHoaDonDto dto)
        {
            if (ModelState.IsValid)
            {
                // Lấy đơn giá từ bảng sản phẩm
                var sp = await _context.SanPhams.FindAsync(dto.MaSp);
                if (sp == null)
                {
                    ModelState.AddModelError("MaSp", "Sản phẩm không tồn tại");
                }
                else
                {
                    var entity = new ChiTietHoaDon
                    {
                        MaHd = dto.MaHd,
                        MaSp = dto.MaSp,
                        SoLuong = dto.SoLuong,
                        DonGia = sp.DonGia, // lấy từ sản phẩm
                        ThanhTien = sp.DonGia * dto.SoLuong // tự tính
                    };

                    _context.Add(entity);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            // Nếu lỗi thì load lại SelectList
            ViewData["MaHd"] = new SelectList(_context.HoaDons, "MaHd", "MaHd", dto.MaHd);
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "TenSp", dto.MaSp);
            return View(dto);
        }
        



        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var entity = await _context.ChiTietHoaDons
                .FirstOrDefaultAsync(x => x.MaCthd == id);

            if (entity == null)
                return NotFound();

            var dto = new ChiTietHoaDonDto
            {
                MaCthd = entity.MaCthd,
                MaHd = entity.MaHd,
                MaSp = entity.MaSp,
                SoLuong = entity.SoLuong,
                DonGia = entity.DonGia,
                ThanhTien = entity.ThanhTien
            };

            ViewData["MaHd"] = new SelectList(_context.HoaDons, "MaHd", "MaHd", dto.MaHd);
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "TenSp", dto.MaSp);

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ChiTietHoaDonDto dto)
        {
            if (id != dto.MaCthd)
                return NotFound();

            if (ModelState.IsValid)
            {
                var entity = await _context.ChiTietHoaDons.FindAsync(id);
                if (entity == null)
                    return NotFound();

                // Chỉ cho sửa số lượng
                entity.SoLuong = dto.SoLuong;

                // Lấy đơn giá gốc từ CSDL (không cho user sửa)
                var sp = await _context.SanPhams.FindAsync(entity.MaSp);
                if (sp != null)
                {
                    entity.DonGia = sp.DonGia;
                    entity.ThanhTien = sp.DonGia * dto.SoLuong;
                }

                _context.Update(entity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaHd"] = new SelectList(_context.HoaDons, "MaHd", "MaHd", dto.MaHd);
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "TenSp", dto.MaSp);
            return View(dto);
        }



        // GET: ChiTietHoaDons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietHoaDon = await _context.ChiTietHoaDons
                .Include(c => c.MaHdNavigation)
                .Include(c => c.MaSpNavigation)
                .FirstOrDefaultAsync(m => m.MaCthd == id);
            if (chiTietHoaDon == null)
            {
                return NotFound();
            }

            return View(chiTietHoaDon);
        }

        // POST: ChiTietHoaDons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chiTietHoaDon = await _context.ChiTietHoaDons.FindAsync(id);
            if (chiTietHoaDon != null)
            {
                _context.ChiTietHoaDons.Remove(chiTietHoaDon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChiTietHoaDonExists(int id)
        {
            return _context.ChiTietHoaDons.Any(e => e.MaCthd == id);
        }
    }
}
