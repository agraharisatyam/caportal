using caportal.Filters;
using Microsoft.AspNetCore.Mvc;

namespace caportal.Areas.Admin.Controllers
{
    public class ClientRequestItem
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = "";
        public string ClientEmail { get; set; } = "";
        public string ClientPhone { get; set; } = "";
        public string City { get; set; } = "";
        public string ServiceRequired { get; set; } = "";
        public string AssignedCA { get; set; } = "Unassigned";
        public string Status { get; set; } = "Pending"; // Pending, Assigned, Completed
        public DateTime RequestedOn { get; set; } = DateTime.UtcNow;
        public string Description { get; set; } = "";
    }

    [Area("Admin")]
    [AdminAuthorize]
    public class RequestsController : Controller
    {
        private static readonly List<ClientRequestItem> _requests = new()
        {
            new ClientRequestItem { Id=1, ClientName="Rahul Sharma", ClientEmail="rahul@techventures.in", ClientPhone="+91 98765 11111", City="Mumbai", ServiceRequired="GST Return Filing", AssignedCA="CA Priya Mehta", Status="Assigned", RequestedOn=DateTime.Now.AddHours(-3), Description="Need urgent GSTR-1 and GSTR-3B filing for Q2." },
            new ClientRequestItem { Id=2, ClientName="Amit Verma", ClientEmail="amit.v@gmail.com", ClientPhone="+91 98765 22222", City="Delhi", ServiceRequired="Income Tax Return (ITR)", AssignedCA="CA Rajesh Sharma", Status="Pending", RequestedOn=DateTime.Now.AddHours(-5), Description="ITR-3 filing for proprietorship business." },
            new ClientRequestItem { Id=3, ClientName="Sneha Gupta", ClientEmail="sneha@greenleaf.in", ClientPhone="+91 98765 33333", City="Bangalore", ServiceRequired="Private Limited Incorporation", AssignedCA="CA Anita Krishnan", Status="Completed", RequestedOn=DateTime.Now.AddDays(-1), Description="Need DIN, DSC, and name approval for new startup." },
            new ClientRequestItem { Id=4, ClientName="Karan Mehta", ClientEmail="karan@sunrise.in", ClientPhone="+91 98765 44444", City="Pune", ServiceRequired="Statutory Audit", AssignedCA="CA Vikram Joshi", Status="Assigned", RequestedOn=DateTime.Now.AddDays(-2), Description="Annual financial audit for FY 2025-26." },
            new ClientRequestItem { Id=5, ClientName="Priya Singh", ClientEmail="priya@skyhigh.in", ClientPhone="+91 98765 55555", City="Ahmedabad", ServiceRequired="Trademark Registration", AssignedCA="Unassigned", Status="Pending", RequestedOn=DateTime.Now.AddDays(-3), Description="Brand name logo trademark search and filing." }
        };

        // GET /Admin/Requests
        [HttpGet]
        public IActionResult Index(string? status)
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            ViewBag.CurrentStatus = status ?? "All";

            var items = _requests.AsEnumerable();
            if (!string.IsNullOrEmpty(status) && !status.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                items = items.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            ViewBag.TotalCount = _requests.Count;
            ViewBag.PendingCount = _requests.Count(r => r.Status == "Pending");
            ViewBag.AssignedCount = _requests.Count(r => r.Status == "Assigned");
            ViewBag.CompletedCount = _requests.Count(r => r.Status == "Completed");

            return View(items.OrderByDescending(r => r.RequestedOn).ToList());
        }

        // POST /Admin/Requests/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int id, string status, string? assignedCa)
        {
            var req = _requests.FirstOrDefault(r => r.Id == id);
            if (req != null)
            {
                req.Status = status;
                if (!string.IsNullOrEmpty(assignedCa))
                    req.AssignedCA = assignedCa;
                TempData["Success"] = $"Request #{id} updated to {status}.";
            }
            return RedirectToAction("Index");
        }

        // POST /Admin/Requests/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var req = _requests.FirstOrDefault(r => r.Id == id);
            if (req != null)
            {
                _requests.Remove(req);
                TempData["Success"] = $"Request #{id} deleted.";
            }
            return RedirectToAction("Index");
        }
    }
}
