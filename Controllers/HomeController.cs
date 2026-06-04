using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Data;
using TicketSystem.Models;

namespace TicketSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");

            ViewBag.IsAdmin = isAdmin;
            ViewBag.UserName = user?.FullName ?? "Kullanıcı";

            // İstatistikler - LINQ ile filtreleme
            if (isAdmin)
            {
                ViewBag.TotalTickets = await _context.Tickets.CountAsync();
                ViewBag.OpenTickets = await _context.Tickets
                    .CountAsync(t => t.Status == TicketStatus.Acik);
                ViewBag.SolvedTickets = await _context.Tickets
                    .CountAsync(t => t.Status == TicketStatus.Cozuldu);
                ViewBag.ClosedTickets = await _context.Tickets
                    .CountAsync(t => t.Status == TicketStatus.Kapandi);
            }
            else
            {
                var userId = _userManager.GetUserId(User);
                ViewBag.TotalTickets = await _context.Tickets
                    .CountAsync(t => t.CustomerId == userId);
                ViewBag.OpenTickets = await _context.Tickets
                    .CountAsync(t => t.CustomerId == userId && t.Status == TicketStatus.Acik);
                ViewBag.SolvedTickets = await _context.Tickets
                    .CountAsync(t => t.CustomerId == userId && t.Status == TicketStatus.Cozuldu);
                ViewBag.ClosedTickets = await _context.Tickets
                    .CountAsync(t => t.CustomerId == userId && t.Status == TicketStatus.Kapandi);
            }

            return View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }
    }
}
