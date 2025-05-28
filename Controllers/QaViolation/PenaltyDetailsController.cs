using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DETP.data;
using DETP.model.QaViolation;
using DETP.auth;

namespace DETP.Controllers.QaViolation
{
    [Authorize]
    public class PenaltyDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PenaltyDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PenaltyDetails.Include(p => p.Category).Include(p => p.SubCategory);
            return View(await applicationDbContext.ToListAsync());
        }


        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var penaltyDetail = await _context.PenaltyDetails
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (penaltyDetail == null)
            {
                return NotFound();
            }

            return View(penaltyDetail);
        }


        public IActionResult Create()
        {
            ViewData["QaViolationCategoryId"] = new SelectList(_context.QaViolationCategories, "Id", "Name");
            ViewData["QaViolationSubCategoryId"] = new SelectList(_context.QaViolationSubCategories, "Id", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FinancialPenalty,Administrative,QaViolationCategoryId,QaViolationSubCategoryId,Max_amount")] PenaltyDetail penaltyDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(penaltyDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["QaViolationCategoryId"] = new SelectList(_context.QaViolationCategories, "Id", "Name", penaltyDetail.QaViolationCategoryId);
            ViewData["QaViolationSubCategoryId"] = new SelectList(_context.QaViolationSubCategories, "Id", "Name", penaltyDetail.QaViolationSubCategoryId);
            return View(penaltyDetail);
        }


        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var penaltyDetail = await _context.PenaltyDetails.FindAsync(id);
            if (penaltyDetail == null)
            {
                return NotFound();
            }
            ViewData["QaViolationCategoryId"] = new SelectList(_context.QaViolationCategories, "Id", "Name", penaltyDetail.QaViolationCategoryId);
            ViewData["QaViolationSubCategoryId"] = new SelectList(_context.QaViolationSubCategories, "Id", "Name", penaltyDetail.QaViolationSubCategoryId);
            return View(penaltyDetail);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FinancialPenalty,Administrative,QaViolationCategoryId,QaViolationSubCategoryId,Max_amount")] PenaltyDetail penaltyDetail)
        {
            if (id != penaltyDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(penaltyDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PenaltyDetailExists(penaltyDetail.Id))
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
            ViewData["QaViolationCategoryId"] = new SelectList(_context.QaViolationCategories, "Id", "Name", penaltyDetail.QaViolationCategoryId);
            ViewData["QaViolationSubCategoryId"] = new SelectList(_context.QaViolationSubCategories, "Id", "Name", penaltyDetail.QaViolationSubCategoryId);
            return View(penaltyDetail);
        }


        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var penaltyDetail = await _context.PenaltyDetails
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (penaltyDetail == null)
            {
                return NotFound();
            }

            return View(penaltyDetail);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var penaltyDetail = await _context.PenaltyDetails.FindAsync(id);
            _context.PenaltyDetails.Remove(penaltyDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult BySubCategory(long id)
        {
            var paneltyDetail = _context.PenaltyDetails.Where(x => x.QaViolationSubCategoryId == id).FirstOrDefault();
            if(paneltyDetail == null)
            {
                return NotFound(Json(new { Message = "Not Found" }));
            }

            return Ok(paneltyDetail);
        }

        private bool PenaltyDetailExists(long id)
        {
            return _context.PenaltyDetails.Any(e => e.Id == id);
        }
    }
}
