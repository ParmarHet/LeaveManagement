using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeavePro.Data;
using LeavePro.Models;
using LeavePro.Constants;

namespace LeavePro.Controllers;

[Authorize(Roles = Roles.Admin)]
public class LeaveTypesController : Controller
{
    private readonly ApplicationDbContext _context;

    public LeaveTypesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: LeaveTypes
    public async Task<IActionResult> Index()
    {
        return View(await _context.LeaveTypes.ToListAsync());
    }

    // GET: LeaveTypes/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: LeaveTypes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Code,DefaultDays,IsPaid,RequiresApproval,MaxConsecutiveDays,YearlyLimit,CarryForward,IsEnabled")] LeaveType leaveType)
    {
        if (ModelState.IsValid)
        {
            leaveType.DateCreated = DateTime.UtcNow;
            leaveType.DateModified = DateTime.UtcNow;
            _context.Add(leaveType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(leaveType);
    }

    // GET: LeaveTypes/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var leaveType = await _context.LeaveTypes.FindAsync(id);
        if (leaveType == null) return NotFound();
        return View(leaveType);
    }

    // POST: LeaveTypes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Code,DefaultDays,IsPaid,RequiresApproval,MaxConsecutiveDays,YearlyLimit,CarryForward,IsEnabled,DateCreated")] LeaveType leaveType)
    {
        if (id != leaveType.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var existing = await _context.LeaveTypes.AsNoTracking().FirstOrDefaultAsync(lt => lt.Id == id);
                if (existing == null) return NotFound();

                int difference = leaveType.DefaultDays - existing.DefaultDays;

                leaveType.DateModified = DateTime.UtcNow;
                _context.Update(leaveType);

                if (difference != 0)
                {
                    var period = DateTime.Now.Year;
                    var allocationsToUpdate = await _context.LeaveAllocations
                        .Where(a => a.LeaveTypeId == id && a.Period == period)
                        .ToListAsync();

                    foreach (var alloc in allocationsToUpdate)
                    {
                        alloc.NumberOfDays += difference;
                        if (alloc.NumberOfDays < 0) alloc.NumberOfDays = 0;
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeaveTypeExists(leaveType.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(leaveType);
    }

    // GET: LeaveTypes/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var leaveType = await _context.LeaveTypes.FirstOrDefaultAsync(m => m.Id == id);
        if (leaveType == null) return NotFound();

        return View(leaveType);
    }

    // POST: LeaveTypes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var leaveType = await _context.LeaveTypes.FindAsync(id);
        if (leaveType != null)
        {
            _context.LeaveTypes.Remove(leaveType);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool LeaveTypeExists(int id)
    {
        return _context.LeaveTypes.Any(e => e.Id == id);
    }
}
