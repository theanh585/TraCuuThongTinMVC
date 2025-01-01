using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TraCuuThongTinMVC.Data;
using Microsoft.AspNetCore.Authorization;
using TraCuuThongTinMVC.ViewModels;

namespace TraCuuThongTinMVC.Controllers
{
    [Authorize]
    public class InfSchesController : Controller
    {
        private readonly SchoolContext _context;

        public InfSchesController(SchoolContext context)
        {
            _context = context;
        }

        // GET: InfSches
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewData["SearchTerm"] = searchTerm;

            var schools = from s in _context.InfSches
                          select s;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                schools = schools.Where(s => s.ScNm.Contains(searchTerm));
            }
            return View(await schools.ToListAsync());
        }

        // GET: InfSches/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var infSch = await _context.InfSches
                .FirstOrDefaultAsync(m => m.ScId == id);
            if (infSch == null)
            {
                return NotFound();
            }

            var professions = await _context.ProfessionDbs
                                    .Where(p => p.ScId == id)
                                    .ToListAsync();
            var viewModel = new SchoolDetailsViewModel
            {
                School = infSch,
                Professions = professions
            };
            return View(viewModel);
        }

        // GET: InfSches/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: InfSches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScId,ScNm,ScArea,ScAddress,ScAddress1,ScTcd,Tel,Fax,Count,Mail,Website,HiuTruong,HiuPho,NguoiChiuTrachNhiemPhapLuat,Image")] InfSch infSch, IFormFile imageFile)
        {
            ModelState.Remove("Image");
            ModelState.Remove("ScId");
            ModelState.Remove("imageFile");

            if (string.IsNullOrWhiteSpace(infSch.ScNm))
            {
                ModelState.AddModelError("ScNm", "Tên Trường không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(infSch.ScAddress))
            {
                ModelState.AddModelError("ScAddress", "Địa chỉ chính không được để trống.");
            }

            if (ModelState.IsValid)
            {   // Tạo ScId tự động
                infSch.ScId = GenerateScId();

                // Kiểm tra trùng lặp ScId trong cơ sở dữ liệu
                while (_context.InfSches.Any(e => e.ScId == infSch.ScId))
                {
                    // Nếu trùng lặp, tạo lại ScId
                    infSch.ScId = GenerateScId();
                }
                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(memoryStream);
                        infSch.Image = memoryStream.ToArray(); // Chuyển đổi hình ảnh thành byte[]
                    }
                }

                _context.Add(infSch);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = infSch.ScId });
            }
            return View(infSch);
        }

        private string GenerateScId()
        {
            return Guid.NewGuid().ToString();
        }
        // GET: InfSches/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var infSch = await _context.InfSches.FindAsync(id);
            if (infSch == null)
            {
                return NotFound();
            }
            return View(infSch);
        }

        // POST: InfSches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ScId,ScNm,ScArea,ScAddress,ScAddress1,ScTcd,Tel,Fax,Count,Mail,Website,HiuTruong,HiuPho,NguoiChiuTrachNhiemPhapLuat,Image")] InfSch infSch, IFormFile imageFile)
        {
            if (id != infSch.ScId)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(infSch.ScNm))
            {
                ModelState.AddModelError("ScNm", "Tên Trường không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(infSch.ScAddress))
            {
                ModelState.AddModelError("ScAddress", "Địa chỉ chính không được để trống.");
            }

            ModelState.Remove("Image");
            ModelState.Remove("ScId");
            ModelState.Remove("imageFile");
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await imageFile.CopyToAsync(memoryStream);
                            infSch.Image = memoryStream.ToArray(); // Chuyển đổi hình ảnh thành byte[]
                        }
                    }

                    _context.Update(infSch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InfSchExists(infSch.ScId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = infSch.ScId });
            }
            return View(infSch);
        }

        // GET: InfSches/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var infSch = await _context.InfSches
                .FirstOrDefaultAsync(m => m.ScId == id);
            if (infSch == null)
            {
                return NotFound();
            }

            return View(infSch);
        }

        // POST: InfSches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var infSch = await _context.InfSches.FindAsync(id);
            if (infSch != null)
            {
                // Tìm tất cả các bản ghi liên quan trong bảng ProfessionDbs
                var relatedProfessions = _context.ProfessionDbs.Where(p => p.ScId == id);

                // Xóa các bản ghi liên quan trong bảng ProfessionDbs
                _context.ProfessionDbs.RemoveRange(relatedProfessions);

                _context.InfSches.Remove(infSch);

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool InfSchExists(string id)
        {
            return _context.InfSches.Any(e => e.ScId == id);
        }
    }
}
