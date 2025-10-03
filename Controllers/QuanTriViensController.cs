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
    public class QuanTriViensController : Controller
    {
        private readonly QuanLyTapHoaContext _context;

        public QuanTriViensController(QuanLyTapHoaContext context)
        {
            _context = context;
        }

        // GET: QuanTriViens
        public async Task<IActionResult> Index()
        {
            return View(await _context.QuanTriViens.ToListAsync());
        }

        // GET: QuanTriViens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quanTriVien = await _context.QuanTriViens
                .FirstOrDefaultAsync(m => m.MaQtv == id);
            if (quanTriVien == null)
            {
                return NotFound();
            }

            return View(quanTriVien);
        }

        // GET: QuanTriViens/Create
        public IActionResult Create()
        {
            var model = new QuanTriVien
            {
                TrangThai = true // default true
            };
            return View(model);
        }

        // POST: QuanTriViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaQtv,HoTen,Email,MatKhau,DienThoai,DiaChi,TrangThai")] QuanTriVien quanTriVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(quanTriVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(quanTriVien);
        }

        // GET: QuanTriViens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quanTriVien = await _context.QuanTriViens.FindAsync(id);
            if (quanTriVien == null)
            {
                return NotFound();
            }
            return View(quanTriVien);
        }

        // POST: QuanTriViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaQtv,HoTen,Email,MatKhau,DienThoai,DiaChi,TrangThai")] QuanTriVien quanTriVien)
        {
            if (id != quanTriVien.MaQtv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quanTriVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuanTriVienExists(quanTriVien.MaQtv))
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
            return View(quanTriVien);
        }

        // GET: QuanTriViens/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quanTriVien = await _context.QuanTriViens
                .FirstOrDefaultAsync(m => m.MaQtv == id);
            if (quanTriVien == null)
            {
                return NotFound();
            }

            return View(quanTriVien);
        }

        // POST: QuanTriViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quanTriVien = await _context.QuanTriViens.FindAsync(id);
            if (quanTriVien != null)
            {
                _context.QuanTriViens.Remove(quanTriVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuanTriVienExists(int id)
        {
            return _context.QuanTriViens.Any(e => e.MaQtv == id);
        }
    }
}
