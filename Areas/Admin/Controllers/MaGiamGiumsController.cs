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
    public class MaGiamGiumsController : Controller
    {
        private readonly QuanLyTapHoaContext _context;

        public MaGiamGiumsController(QuanLyTapHoaContext context)
        {
            _context = context;
        }

        // GET: MaGiamGiums
        public async Task<IActionResult> Index()
        {
            return View(await _context.MaGiamGia.ToListAsync());
        }

        // GET: MaGiamGiums/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maGiamGium = await _context.MaGiamGia
                .FirstOrDefaultAsync(m => m.MaVoucher == id);
            if (maGiamGium == null)
            {
                return NotFound();
            }

            return View(maGiamGium);
        }

        // GET: MaGiamGiums/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MaGiamGiums/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaVoucher,TenVoucher,MoTa,PhanTramGiam,GiamTienTrucTiep,GiaTriDonToiThieu,SoLuong,DaSuDung,NgayBatDau,NgayKetThuc,TrangThai")] MaGiamGium maGiamGium)
        {
            if (ModelState.IsValid)
            {
                _context.Add(maGiamGium);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(maGiamGium);
        }

        // GET: MaGiamGiums/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maGiamGium = await _context.MaGiamGia.FindAsync(id);
            if (maGiamGium == null)
            {
                return NotFound();
            }
            return View(maGiamGium);
        }

        // POST: MaGiamGiums/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaVoucher,TenVoucher,MoTa,PhanTramGiam,GiamTienTrucTiep,GiaTriDonToiThieu,SoLuong,DaSuDung,NgayBatDau,NgayKetThuc,TrangThai")] MaGiamGium maGiamGium)
        {
            if (id != maGiamGium.MaVoucher)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(maGiamGium);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaGiamGiumExists(maGiamGium.MaVoucher))
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
            return View(maGiamGium);
        }

        // GET: MaGiamGiums/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maGiamGium = await _context.MaGiamGia
                .FirstOrDefaultAsync(m => m.MaVoucher == id);
            if (maGiamGium == null)
            {
                return NotFound();
            }

            return View(maGiamGium);
        }

        // POST: MaGiamGiums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var maGiamGium = await _context.MaGiamGia.FindAsync(id);
            if (maGiamGium != null)
            {
                _context.MaGiamGia.Remove(maGiamGium);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaGiamGiumExists(string id)
        {
            return _context.MaGiamGia.Any(e => e.MaVoucher == id);
        }
    }
}