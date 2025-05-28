using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DETP.data;
using DETP.model;
using DETP.auth;

namespace DETP.Controllers
{
    [Authorize]
    public class DivisionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DivisionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Divisions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Divisions.Include(x=>x.DivisionHead).ToListAsync());
        }

        // GET: Divisions/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var division = await _context.Divisions
                .FirstOrDefaultAsync(m => m.id == id);
            if (division == null)
            {
                return NotFound();
            }

            return View(division);
        }

        // GET: Divisions/Create
        public IActionResult Create()
        {
            ViewBag.Users = _context.Users.ToList();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Division division)
        {
            if (ModelState.IsValid)
            {
                _context.Add(division);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = _context.Users.ToList();
            return View(division);
        }


        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var division = await _context.Divisions.FindAsync(id);
            ViewBag.Users = _context.Users.ToList();
            if (division == null)
            {
                return NotFound();
            }
            return View(division);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Division division)
        {
            if (id != division.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(division);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DivisionExists(division.id))
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
            ViewBag.Users = _context.Users.ToList();
            return View(division);
        }


        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var division = await _context.Divisions
                .FirstOrDefaultAsync(m => m.id == id);
            if (division == null)
            {
                return NotFound();
            }

            return View(division);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var division = await _context.Divisions.FindAsync(id);
            _context.Divisions.Remove(division);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DivisionExists(long id)
        {
            return _context.Divisions.Any(e => e.id == id);
        }


        [HttpGet]
        public async Task<List<Division>> AllDivision()
        {
            return await _context.Divisions.ToListAsync();
        }
    }
}
