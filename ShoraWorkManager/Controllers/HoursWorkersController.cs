using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Persistence.Models;

namespace ShoraWorkManager.Controllers
{
    public class HoursWorkersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoursWorkersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HoursWorkers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ContructionSiteWorkedHoursWorkers.Include(c => c.ConstructionSite).Include(c => c.Worker);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: HoursWorkers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contructionSiteWorkedHoursWorker = await _context.ContructionSiteWorkedHoursWorkers
                .Include(c => c.ConstructionSite)
                .Include(c => c.Worker)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contructionSiteWorkedHoursWorker == null)
            {
                return NotFound();
            }

            return View(contructionSiteWorkedHoursWorker);
        }

        // GET: HoursWorkers/Create
        public IActionResult Create()
        {
            ViewData["ConstructionSiteId"] = new SelectList(_context.ConstructionSites, "Id", "Description");
            ViewData["WorkerId"] = new SelectList(_context.Workers, "Id", "Name");
            return View();
        }

        // POST: HoursWorkers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ConstructionSiteId,WorkerId,WorkedHours,RegisteredAt,WasPayed")] ContructionSiteWorkedHoursWorker contructionSiteWorkedHoursWorker)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contructionSiteWorkedHoursWorker);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConstructionSiteId"] = new SelectList(_context.ConstructionSites, "Id", "Description", contructionSiteWorkedHoursWorker.ConstructionSiteId);
            ViewData["WorkerId"] = new SelectList(_context.Workers, "Id", "Name", contructionSiteWorkedHoursWorker.WorkerId);
            return View(contructionSiteWorkedHoursWorker);
        }

        // GET: HoursWorkers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contructionSiteWorkedHoursWorker = await _context.ContructionSiteWorkedHoursWorkers.FindAsync(id);
            if (contructionSiteWorkedHoursWorker == null)
            {
                return NotFound();
            }
            ViewData["ConstructionSiteId"] = new SelectList(_context.ConstructionSites, "Id", "Description", contructionSiteWorkedHoursWorker.ConstructionSiteId);
            ViewData["WorkerId"] = new SelectList(_context.Workers, "Id", "Name", contructionSiteWorkedHoursWorker.WorkerId);
            return View(contructionSiteWorkedHoursWorker);
        }

        // POST: HoursWorkers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ConstructionSiteId,WorkerId,WorkedHours,RegisteredAt,WasPayed")] ContructionSiteWorkedHoursWorker contructionSiteWorkedHoursWorker)
        {
            if (id != contructionSiteWorkedHoursWorker.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contructionSiteWorkedHoursWorker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContructionSiteWorkedHoursWorkerExists(contructionSiteWorkedHoursWorker.Id))
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
            ViewData["ConstructionSiteId"] = new SelectList(_context.ConstructionSites, "Id", "Description", contructionSiteWorkedHoursWorker.ConstructionSiteId);
            ViewData["WorkerId"] = new SelectList(_context.Workers, "Id", "Name", contructionSiteWorkedHoursWorker.WorkerId);
            return View(contructionSiteWorkedHoursWorker);
        }

        // GET: HoursWorkers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contructionSiteWorkedHoursWorker = await _context.ContructionSiteWorkedHoursWorkers
                .Include(c => c.ConstructionSite)
                .Include(c => c.Worker)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contructionSiteWorkedHoursWorker == null)
            {
                return NotFound();
            }

            return View(contructionSiteWorkedHoursWorker);
        }

        // POST: HoursWorkers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contructionSiteWorkedHoursWorker = await _context.ContructionSiteWorkedHoursWorkers.FindAsync(id);
            if (contructionSiteWorkedHoursWorker != null)
            {
                _context.ContructionSiteWorkedHoursWorkers.Remove(contructionSiteWorkedHoursWorker);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContructionSiteWorkedHoursWorkerExists(int id)
        {
            return _context.ContructionSiteWorkedHoursWorkers.Any(e => e.Id == id);
        }
    }
}
