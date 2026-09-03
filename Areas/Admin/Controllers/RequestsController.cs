using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using caportal.Data;
using caportal.Filters;
using caportal.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace caportal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class RequestsController : Controller
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public RequestsController(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        // GET /Admin/Requests
        [HttpGet]
        public async Task<IActionResult> Index(string? status)
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            ViewBag.CurrentStatus = status ?? "All";

            using var db = _dbFactory.CreateDbContext();
            var allRequests = await db.ClientRequests.OrderByDescending(r => r.RequestedOn).ToListAsync();

            ViewBag.TotalCount = allRequests.Count;
            ViewBag.PendingCount = allRequests.Count(r => r.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase));
            ViewBag.AssignedCount = allRequests.Count(r => r.Status.Equals("Assigned", StringComparison.OrdinalIgnoreCase));
            ViewBag.CompletedCount = allRequests.Count(r => r.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase));

            var filtered = allRequests.AsEnumerable();
            if (!string.IsNullOrEmpty(status) && !status.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                filtered = filtered.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            // Also load active CAs for quick assignment dropdown in view
            ViewBag.ActiveCAs = await db.CaProfessionals.Where(c => c.Status == "Active").OrderBy(c => c.Name).Select(c => c.Name).ToListAsync();

            return View(filtered.ToList());
        }

        // POST /Admin/Requests/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status, string? assignedCa)
        {
            using var db = _dbFactory.CreateDbContext();
            var req = await db.ClientRequests.FindAsync(id);
            if (req != null)
            {
                req.Status = status;
                if (!string.IsNullOrEmpty(assignedCa))
                    req.AssignedCA = assignedCa;
                await db.SaveChangesAsync();
                TempData["Success"] = $"Request #{id} updated to {status}.";
            }
            return RedirectToAction("Index");
        }

        // POST /Admin/Requests/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            using var db = _dbFactory.CreateDbContext();
            var req = await db.ClientRequests.FindAsync(id);
            if (req != null)
            {
                db.ClientRequests.Remove(req);
                await db.SaveChangesAsync();
                TempData["Success"] = $"Request #{id} deleted successfully.";
            }
            return RedirectToAction("Index");
        }

        // GET /Admin/Requests/ExportCsv
        [HttpGet]
        public async Task<IActionResult> ExportCsv()
        {
            using var db = _dbFactory.CreateDbContext();
            var requests = await db.ClientRequests.OrderByDescending(r => r.RequestedOn).ToListAsync();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("ID,Client Name,Email,Phone,City,Client Type,Service Required,Assigned CA,Status,Source,Description,Requested On");

            foreach (var r in requests)
            {
                var cleanDesc = (r.Description ?? "").Replace("\"", "\"\"").Replace("\r", " ").Replace("\n", " ");
                sb.AppendLine($"\"{r.Id}\",\"{r.ClientName}\",\"{r.ClientEmail}\",\"{r.ClientPhone}\",\"{r.City}\",\"{r.ClientType}\",\"{r.ServiceRequired}\",\"{r.AssignedCA}\",\"{r.Status}\",\"{r.Source}\",\"{cleanDesc}\",\"{r.RequestedOn:yyyy-MM-dd HH:mm:ss}\"");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"CACampus_Leads_{DateTime.UtcNow:yyyyMMdd_HHmm}.csv");
        }
    }
}
