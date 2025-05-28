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
    public class QaViolationSubCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QaViolationSubCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.QaViolationSubCategories.Include(q => q.Category);
            return View(await applicationDbContext.ToListAsync());
        }


        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var qaViolationSubCategory = await _context.QaViolationSubCategories
                .Include(q => q.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (qaViolationSubCategory == null)
            {
                return NotFound();
            }

            return View(qaViolationSubCategory);
        }


        public IActionResult Create()
        {
            ViewData["QaViolationCategoryNames"] = new SelectList(_context.QaViolationCategories, "Id", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,QaViolationCategoryId")] QaViolationSubCategory qaViolationSubCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(qaViolationSubCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["QaViolationCategoryId"] = new SelectList(_context.QaViolationCategories, "Id", "Id", qaViolationSubCategory.QaViolationCategoryId);
            return View(qaViolationSubCategory);
        }


        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var qaViolationSubCategory = await _context.QaViolationSubCategories.FindAsync(id);
            if (qaViolationSubCategory == null)
            {
                return NotFound();
            }
            ViewData["QaViolationCategoryId"] = new SelectList(_context.QaViolationCategories, "Id", "Name", qaViolationSubCategory.QaViolationCategoryId);
            return View(qaViolationSubCategory);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,QaViolationCategoryId")] QaViolationSubCategory qaViolationSubCategory)
        {
            if (id != qaViolationSubCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(qaViolationSubCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QaViolationSubCategoryExists(qaViolationSubCategory.Id))
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
            ViewData["QaViolationCategoryId"] = new SelectList(_context.QaViolationCategories, "Id", "Name", qaViolationSubCategory.QaViolationCategoryId);
            return View(qaViolationSubCategory);
        }


        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var qaViolationSubCategory = await _context.QaViolationSubCategories
                .Include(q => q.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (qaViolationSubCategory == null)
            {
                return NotFound();
            }

            return View(qaViolationSubCategory);
        }


        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var qaViolationSubCategory = await _context.QaViolationSubCategories.FindAsync(id);
            _context.QaViolationSubCategories.Remove(qaViolationSubCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IEnumerable<QaViolationSubCategory> ByCategory(long id)
        {
            var subCategory = _context.QaViolationSubCategories.Where(x => x.QaViolationCategoryId == id).ToList();
            return subCategory;
        }

        private bool QaViolationSubCategoryExists(long id)
        {
            return _context.QaViolationSubCategories.Any(e => e.Id == id);
        }
    }
}
