using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    public class OrdersController : Controller
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public OrdersController(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        // GET /Admin/Orders
        [HttpGet]
        public async Task<IActionResult> Index(string? status, string? paymentStatus)
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            ViewBag.CurrentStatus = status ?? "All";
            ViewBag.CurrentPaymentStatus = paymentStatus ?? "All";

            using var db = _dbFactory.CreateDbContext();

            var allOrders = await db.DashboardOrders.OrderByDescending(o => o.CreatedAt).ToListAsync();

            ViewBag.TotalOrders = allOrders.Count;
            ViewBag.TotalRevenue = allOrders.Sum(o => o.AmountValue);
            ViewBag.CompletedOrders = allOrders.Count(o => o.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase));
            ViewBag.PendingOrders = allOrders.Count(o => !o.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase));
            ViewBag.AvgOrderValue = allOrders.Any() ? allOrders.Average(o => o.AmountValue) : 0;

            var filtered = allOrders.AsEnumerable();

            if (!string.IsNullOrEmpty(status) && !status.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                filtered = filtered.Where(o => o.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(paymentStatus) && !paymentStatus.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                filtered = filtered.Where(o => o.PaymentStatus.Equals(paymentStatus, StringComparison.OrdinalIgnoreCase));
            }

            ViewBag.ActiveCAs = await db.CaProfessionals.Where(c => c.Status == "Active").OrderBy(c => c.Name).Select(c => c.Name).ToListAsync();
            ViewBag.CoveredServices = await db.CoveredServices.OrderBy(s => s.DisplayOrder).Select(s => s.Title).ToListAsync();

            return View(filtered.ToList());
        }

        // POST /Admin/Orders/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(DashboardOrder model)
        {
            using var db = _dbFactory.CreateDbContext();

            if (model.Id == 0)
            {
                // Create New Order
                if (string.IsNullOrWhiteSpace(model.OrderId))
                {
                    model.OrderId = $"ORD-{DateTime.UtcNow:yyMMdd}{new Random().Next(100, 999)}";
                }

                model.Amount = $"₹ {model.AmountValue:N0}";
                model.ColorClass = model.Status switch
                {
                    "Completed" => "bg-success",
                    "Document Pending" => "bg-info",
                    _ => "bg-warning"
                };
                model.CreatedAt = DateTime.UtcNow;

                db.DashboardOrders.Add(model);
                await db.SaveChangesAsync();
                TempData["Success"] = $"Order {model.OrderId} created successfully!";
            }
            else
            {
                // Edit Existing
                var existing = await db.DashboardOrders.FindAsync(model.Id);
                if (existing != null)
                {
                    existing.Customer = model.Customer;
                    existing.ClientEmail = model.ClientEmail ?? string.Empty;
                    existing.ClientPhone = model.ClientPhone ?? string.Empty;
                    existing.Service = model.Service;
                    existing.AmountValue = model.AmountValue;
                    existing.Amount = $"₹ {model.AmountValue:N0}";
                    existing.Status = model.Status;
                    existing.PaymentStatus = model.PaymentStatus;
                    existing.AssignedCA = model.AssignedCA ?? string.Empty;
                    existing.ColorClass = model.Status switch
                    {
                        "Completed" => "bg-success",
                        "Document Pending" => "bg-info",
                        _ => "bg-warning"
                    };

                    await db.SaveChangesAsync();
                    TempData["Success"] = $"Order {existing.OrderId} updated successfully!";
                }
            }

            return RedirectToAction("Index");
        }

        // POST /Admin/Orders/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status, string paymentStatus, string? assignedCa)
        {
            using var db = _dbFactory.CreateDbContext();
            var order = await db.DashboardOrders.FindAsync(id);
            if (order != null)
            {
                if (!string.IsNullOrEmpty(status)) order.Status = status;
                if (!string.IsNullOrEmpty(paymentStatus)) order.PaymentStatus = paymentStatus;
                if (!string.IsNullOrEmpty(assignedCa)) order.AssignedCA = assignedCa;

                order.ColorClass = order.Status switch
                {
                    "Completed" => "bg-success",
                    "Document Pending" => "bg-info",
                    _ => "bg-warning"
                };

                await db.SaveChangesAsync();
                TempData["Success"] = $"Order {order.OrderId} status updated!";
            }
            return RedirectToAction("Index");
        }

        // POST /Admin/Orders/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            using var db = _dbFactory.CreateDbContext();
            var order = await db.DashboardOrders.FindAsync(id);
            if (order != null)
            {
                db.DashboardOrders.Remove(order);
                await db.SaveChangesAsync();
                TempData["Success"] = $"Order {order.OrderId} deleted successfully.";
            }
            return RedirectToAction("Index");
        }

        // GET /Admin/Orders/ExportCsv
        [HttpGet]
        public async Task<IActionResult> ExportCsv()
        {
            using var db = _dbFactory.CreateDbContext();
            var orders = await db.DashboardOrders.OrderByDescending(o => o.CreatedAt).ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("ID,Order ID,Customer,Email,Phone,Service,Amount (INR),Status,Payment Status,Assigned CA,Created At");

            foreach (var o in orders)
            {
                sb.AppendLine($"\"{o.Id}\",\"{o.OrderId}\",\"{o.Customer}\",\"{o.ClientEmail}\",\"{o.ClientPhone}\",\"{o.Service}\",\"{o.AmountValue}\",\"{o.Status}\",\"{o.PaymentStatus}\",\"{o.AssignedCA}\",\"{o.CreatedAt:yyyy-MM-dd HH:mm:ss}\"");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"CACampus_Orders_{DateTime.UtcNow:yyyyMMdd_HHmm}.csv");
        }
    }
}
