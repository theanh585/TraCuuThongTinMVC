using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TraCuuThongTinMVC.Data;
using TraCuuThongTinMVC.ViewModels;

namespace TraCuuThongTinMVC.Controllers
{
    public class ProfessionDbsController : Controller
    {
        private readonly SchoolContext _context;

        public ProfessionDbsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: ProfessionDbs
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProfessionDbs.ToListAsync());
        }

        // GET: ProfessionDbs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var professionDb = await _context.ProfessionDbs
                .FirstOrDefaultAsync(m => m.Cd == id);
            if (professionDb == null)
            {
                return NotFound();
            }

            return View(professionDb);
        }

        // GET: ProfessionDbs/Create
        public IActionResult Create(string scId)
        {
            if (string.IsNullOrEmpty(scId))
            {
                return NotFound();
            }
            var model = new ProfessionDb
            {
                ScId = scId // Gán ScId từ tham số
            };
            return View(model);
        }

        // POST: ProfessionDbs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Cd,Nm,ScId,Count,MoTa,GvCn")] ProfessionDb professionDb)
        {
            ModelState.Remove("Cd");
            if (ModelState.IsValid)
            {
                professionDb.Cd = Guid.NewGuid().ToString();
                while (_context.ProfessionDbs.Any(e => e.Cd == professionDb.Cd))
                {
                    professionDb.Cd = Guid.NewGuid().ToString();
                }
                _context.Add(professionDb);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "InfSches", new { id = professionDb.ScId });
            }
            return View(professionDb);
        }
        
        // GET: ProfessionDbs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var professionDb = await _context.ProfessionDbs.FindAsync(id);
            if (professionDb == null)
            {
                return NotFound();
            }
            return View(professionDb);
        }

        // POST: ProfessionDbs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Cd,Nm,ScId,Count,MoTa,GvCn")] ProfessionDb professionDb)
        {
            if (id != professionDb.Cd)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(professionDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfessionDbExists(professionDb.Cd))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "InfSches", new { id = professionDb.ScId });
            }
            return View(professionDb);
        }

        // GET: ProfessionDbs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var professionDb = await _context.ProfessionDbs
                .FirstOrDefaultAsync(m => m.Cd == id);
            if (professionDb == null)
            {
                return NotFound();
            }

            return View(professionDb);
        }

        // POST: ProfessionDbs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var professionDb = await _context.ProfessionDbs.FindAsync(id);
            if (professionDb != null)
            {
                _context.ProfessionDbs.Remove(professionDb);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "InfSches", new { id = professionDb.ScId });
        }

        private bool ProfessionDbExists(string id)
        {
            return _context.ProfessionDbs.Any(e => e.Cd == id);
        }
    }
}
