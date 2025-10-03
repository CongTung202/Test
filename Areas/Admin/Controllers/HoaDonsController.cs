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
    public class HoaDonsController : Controller
    {
        private readonly QuanLyTapHoaContext _context;

        public HoaDonsController(QuanLyTapHoaContext context)
        {
            _context = context;
        }

        // GET: HoaDons
        public async Task<IActionResult> Index()
        {
            var quanLyTapHoaContext = _context.HoaDons.Include(h => h.MaKhNavigation).Include(h => h.MaVoucherNavigation);
            return View(await quanLyTapHoaContext.ToListAsync());
        }

        // GET: HoaDons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.MaKhNavigation)
                .Include(h => h.MaVoucherNavigation)
                .FirstOrDefaultAsync(m => m.MaHd == id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.MaKh = new SelectList(_context.KhachHangs, "MaKh", "HoTen");
            ViewBag.MaVoucher = new SelectList(_context.MaGiamGia.Where(v => v.TrangThai == true), "MaVoucher", "MaVoucher");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAjax([FromBody] HoaDonDto dto)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });

            decimal tienGoc = dto.TongTien;
            decimal tienGiam = 0;

            if (!string.IsNullOrEmpty(dto.MaVoucher))
            {
                var voucher = await _context.MaGiamGia
                    .FirstOrDefaultAsync(v => v.MaVoucher == dto.MaVoucher && v.TrangThai == true);

                if (voucher != null
                    && DateTime.Now >= voucher.NgayBatDau
                    && DateTime.Now <= voucher.NgayKetThuc
                    && voucher.SoLuong > voucher.DaSuDung
                    && tienGoc >= (voucher.GiaTriDonToiThieu ?? 0))
                {
                    if (voucher.PhanTramGiam.HasValue)
                    {
                        tienGiam = tienGoc * voucher.PhanTramGiam.Value / 100;
                    }

                    if (voucher.GiamTienTrucTiep.HasValue)
                    {
                        tienGiam += voucher.GiamTienTrucTiep.Value;
                    }

                    if (tienGiam > tienGoc) tienGiam = tienGoc;

                    voucher.DaSuDung += 1;
                }
            }

            var hoaDon = new HoaDon
            {
                NgayLap = DateTime.Now,
                TongTien = tienGoc,
                TienGiamGia = tienGiam,
                ThanhTien = tienGoc - tienGiam,
                MaKh = dto.MaKh,
                MaVoucher = string.IsNullOrEmpty(dto.MaVoucher) ? null : dto.MaVoucher,
                DiaChiGiaoHang = dto.DiaChiGiaoHang,
                SdtgiaoHang = dto.SdtgiaoHang,
                GhiChu = dto.GhiChu,
                TrangThai = dto.TrangThai
            };

            _context.HoaDons.Add(hoaDon);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Thêm hóa đơn thành công!" });
        }

        [HttpGet]
        public IActionResult TinhVoucher(string maVoucher, decimal tongTien)
        {
            var voucher = _context.MaGiamGia.FirstOrDefault(v => v.MaVoucher == maVoucher && v.TrangThai == true);
            if (voucher == null)
                return Json(new { success = false, message = "Voucher không hợp lệ" });

            decimal tienGiam = 0;
            if (voucher.PhanTramGiam.HasValue)
                tienGiam += tongTien * voucher.PhanTramGiam.Value / 100;

            if (voucher.GiamTienTrucTiep.HasValue)
                tienGiam += voucher.GiamTienTrucTiep.Value;

            if (tienGiam > tongTien) tienGiam = tongTien;

            return Json(new { success = true, tienGiam = tienGiam, thanhTien = tongTien - tienGiam });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var hoaDon = await _context.HoaDons.FirstOrDefaultAsync(h => h.MaHd == id);
            if (hoaDon == null) return NotFound();

            var dto = new HoaDonDto
            {
                MaHd = hoaDon.MaHd,
                NgayLap = hoaDon.NgayLap,
                TongTien = hoaDon.TongTien,
                TienGiamGia = hoaDon.TienGiamGia,
                ThanhTien = hoaDon.ThanhTien,
                MaKh = hoaDon.MaKh,
                MaVoucher = hoaDon.MaVoucher,
                DiaChiGiaoHang = hoaDon.DiaChiGiaoHang,
                SdtgiaoHang = hoaDon.SdtgiaoHang,
                GhiChu = hoaDon.GhiChu,
                TrangThai = hoaDon.TrangThai
            };

            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "HoTen", dto.MaKh);
            ViewData["MaVoucher"] = new SelectList(_context.MaGiamGia, "MaVoucher", "MaVoucher", dto.MaVoucher);

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> EditAjax([FromBody] HoaDonDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });

            var hoaDon = await _context.HoaDons.FindAsync(dto.MaHd);
            if (hoaDon == null)
                return NotFound(new { message = "Không tìm thấy hóa đơn" });

            decimal tienGiam = await TinhTienGiam(dto.MaVoucher, dto.TongTien);

            hoaDon.NgayLap = dto.NgayLap == default ? DateTime.Now : dto.NgayLap;
            hoaDon.TongTien = dto.TongTien;
            hoaDon.TienGiamGia = tienGiam;
            hoaDon.ThanhTien = dto.TongTien - tienGiam;
            hoaDon.MaKh = dto.MaKh;
            hoaDon.MaVoucher = string.IsNullOrEmpty(dto.MaVoucher) ? null : dto.MaVoucher;
            hoaDon.DiaChiGiaoHang = dto.DiaChiGiaoHang;
            hoaDon.SdtgiaoHang = dto.SdtgiaoHang;
            hoaDon.GhiChu = dto.GhiChu;
            hoaDon.TrangThai = dto.TrangThai;

            _context.Update(hoaDon);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật thành công" });
        }

        private async Task<decimal> TinhTienGiam(string? maVoucher, decimal tongTien)
        {
            if (string.IsNullOrEmpty(maVoucher)) return 0;

            var voucher = await _context.MaGiamGia.FirstOrDefaultAsync(v => v.MaVoucher == maVoucher && v.TrangThai);
            if (voucher == null) return 0;

            if (DateTime.Now < voucher.NgayBatDau || DateTime.Now > voucher.NgayKetThuc)
                return 0;

            if (tongTien < (voucher.GiaTriDonToiThieu ?? 0))
                return 0;

            decimal tienGiam = 0;
            if (voucher.PhanTramGiam.HasValue)
                tienGiam += tongTien * voucher.PhanTramGiam.Value / 100;

            if (voucher.GiamTienTrucTiep.HasValue)
                tienGiam += voucher.GiamTienTrucTiep.Value;

            if (tienGiam > tongTien) tienGiam = tongTien;

            voucher.DaSuDung += 1;
            await _context.SaveChangesAsync();

            return tienGiam;
        }

        // GET: HoaDons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.MaKhNavigation)
                .Include(h => h.MaVoucherNavigation)
                .FirstOrDefaultAsync(m => m.MaHd == id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        // POST: HoaDons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon != null)
            {
                _context.HoaDons.Remove(hoaDon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HoaDonExists(int id)
        {
            return _context.HoaDons.Any(e => e.MaHd == id);
        }
    }
}