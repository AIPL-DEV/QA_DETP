using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DETP.data;
using DETP.model.QaViolation;

namespace DETP.Controllers.QaViolation
{
    public class QaViolationCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QaViolationCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            return View(await _context.QaViolationCategories.ToListAsync());
        }


        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var qaViolationCategory = await _context.QaViolationCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (qaViolationCategory == null)
            {
                return NotFound();
            }

            return View(qaViolationCategory);
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] QaViolationCategory qaViolationCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(qaViolationCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(qaViolationCategory);
        }


        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var qaViolationCategory = await _context.QaViolationCategories.FindAsync(id);
            if (qaViolationCategory == null)
            {
                return NotFound();
            }
            return View(qaViolationCategory);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name")] QaViolationCategory qaViolationCategory)
        {
            if (id != qaViolationCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(qaViolationCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QaViolationCategoryExists(qaViolationCategory.Id))
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
            return View(qaViolationCategory);
        }


        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var qaViolationCategory = await _context.QaViolationCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (qaViolationCategory == null)
            {
                return NotFound();
            }

            return View(qaViolationCategory);
        }


        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var qaViolationCategory = await _context.QaViolationCategories.FindAsync(id);
            _context.QaViolationCategories.Remove(qaViolationCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QaViolationCategoryExists(long id)
        {
            return _context.QaViolationCategories.Any(e => e.Id == id);
        }
    }
}
