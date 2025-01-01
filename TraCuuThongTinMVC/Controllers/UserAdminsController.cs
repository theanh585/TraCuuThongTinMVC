using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TraCuuThongTinMVC.Data;

namespace TraCuuThongTinMVC.Controllers
{
    public class UserAdminsController : Controller
    {
        private readonly SchoolContext _context;

        public UserAdminsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: UserAdmins
        public async Task<IActionResult> Index()
        {
            return View(await _context.UserAdmins.ToListAsync());
        }

        // GET: UserAdmins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAdmin = await _context.UserAdmins
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (userAdmin == null)
            {
                return NotFound();
            }

            return View(userAdmin);
        }

        // GET: UserAdmins/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserAdmins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,UserNm,Passwork")] UserAdmin userAdmin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userAdmin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userAdmin);
        }

        // GET: UserAdmins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAdmin = await _context.UserAdmins.FindAsync(id);
            if (userAdmin == null)
            {
                return NotFound();
            }
            return View(userAdmin);
        }

        // POST: UserAdmins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,UserNm,Passwork")] UserAdmin userAdmin)
        {
            if (id != userAdmin.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userAdmin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserAdminExists(userAdmin.UserId))
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
            return View(userAdmin);
        }

        // GET: UserAdmins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAdmin = await _context.UserAdmins
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (userAdmin == null)
            {
                return NotFound();
            }

            return View(userAdmin);
        }

        // POST: UserAdmins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userAdmin = await _context.UserAdmins.FindAsync(id);
            if (userAdmin != null)
            {
                _context.UserAdmins.Remove(userAdmin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserAdminExists(int id)
        {
            return _context.UserAdmins.Any(e => e.UserId == id);
        }
    }
}
