using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Data;
using TicketSystem.Models;
using TicketSystem.Models.ViewModels;

namespace TicketSystem.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Ticket/Index - Ticket listesi (LINQ filtreleme)
        public async Task<IActionResult> Index(TicketStatus? filterStatus, TicketPriority? filterPriority, string? searchTerm)
        {
            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            // Temel sorgu - LINQ ile
            IQueryable<Ticket> query = _context.Tickets
                .Include(t => t.Customer)
                .Include(t => t.SupportAgent)
                .Include(t => t.Replies);

            // Müşteri yalnızca kendi taleplerini görür
            if (!isAdmin)
                query = query.Where(t => t.CustomerId == userId);

            // Durum filtresi (Sadece açık talepleri getir vb.)
            if (filterStatus.HasValue)
                query = query.Where(t => t.Status == filterStatus.Value);

            // Öncelik filtresi
            if (filterPriority.HasValue)
                query = query.Where(t => t.Priority == filterPriority.Value);

            // Arama
            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(t => t.Title.Contains(searchTerm) || t.Description.Contains(searchTerm));

            var tickets = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();

            var viewModel = new TicketListViewModel
            {
                Tickets = tickets,
                FilterStatus = filterStatus,
                FilterPriority = filterPriority,
                SearchTerm = searchTerm
            };

            ViewBag.IsAdmin = isAdmin;
            return View(viewModel);
        }

        // GET: /Ticket/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Ticket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTicketViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = _userManager.GetUserId(User)!;
            var ticket = new Ticket
            {
                Title = model.Title,
                Description = model.Description,
                Priority = model.Priority,
                CustomerId = userId,
                Status = TicketStatus.Acik,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Destek talebiniz başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Detail), new { id = ticket.Id });
        }

        // GET: /Ticket/Detail/5
        public async Task<IActionResult> Detail(int id)
        {
            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            // 1. Adım: Önce sadece bileti ve temel ilişkilerini çekiyoruz (Güvenli)
            var ticket = await _context.Tickets
                .Include(t => t.Customer)
                .Include(t => t.SupportAgent)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null) return NotFound();

            // Müşteri yalnızca kendi talebine erişebilir
            if (!isAdmin && ticket.CustomerId != userId)
                return Forbid();

            // 2. Adım: Bilete ait cevapları ve yazarlarını ayrı bir sorguyla, güvenle çekiyoruz
            var replies = await _context.TicketReplies
                .Where(r => r.TicketId == id)
                .Include(r => r.Author)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();

            var viewModel = new TicketDetailViewModel
            {
                Ticket = ticket,
                Replies = replies, // Güvenle doldurulan cevap listesi
                NewReply = new AddReplyViewModel { TicketId = id },
                IsStaff = isAdmin
            };

            ViewBag.IsAdmin = isAdmin;
            return View(viewModel);
        }

        // POST: /Ticket/AddReply
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReply(AddReplyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Cevap metni boş olamaz.";
                return RedirectToAction(nameof(Detail), new { id = model.TicketId });
            }

            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole("Admin");

            var ticket = await _context.Tickets.FindAsync(model.TicketId);
            if (ticket == null) return NotFound();

            if (!isAdmin && ticket.CustomerId != userId) return Forbid();

            var reply = new TicketReply
            {
                TicketId = model.TicketId,
                Content = model.Content,
                AuthorId = userId,
                IsStaffReply = isAdmin,
                CreatedAt = DateTime.UtcNow
            };

            _context.TicketReplies.Add(reply);
            ticket.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cevabınız eklendi.";
            return RedirectToAction(nameof(Detail), new { id = ticket.Id });
        }

        // POST: /Ticket/Assign/5 - Destek ekibi üstlenir
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            var userId = _userManager.GetUserId(User)!;
            ticket.SupportAgentId = userId;
            ticket.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Talep üzerinize atandı.";
            return RedirectToAction(nameof(Detail), new { id });
        }

        // POST: /Ticket/UpdateStatus/5 - Durum güncelleme
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, TicketStatus status)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            ticket.Status = status;
            ticket.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Talep durumu '{GetStatusDisplayName(status)}' olarak güncellendi.";
            return RedirectToAction(nameof(Detail), new { id });
        }

        // GET: /Ticket/MyTickets - Müşterinin kendi talepleri
        public async Task<IActionResult> MyTickets()
        {
            var userId = _userManager.GetUserId(User);

            var openTickets = await _context.Tickets
                .Where(t => t.CustomerId == userId && t.Status == TicketStatus.Acik)
                .Include(t => t.SupportAgent)
                .Include(t => t.Replies)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            var allTickets = await _context.Tickets
                .Where(t => t.CustomerId == userId)
                .Include(t => t.SupportAgent)
                .Include(t => t.Replies)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.OpenTickets = openTickets;
            return View(allTickets);
        }

        // POST: /Ticket/Delete/5 - Sadece Admin Talebi Silebilir
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Replies)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null) return NotFound();

            if (ticket.Replies != null && ticket.Replies.Any())
            {
                _context.TicketReplies.RemoveRange(ticket.Replies);
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Destek talebi başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private static string GetStatusDisplayName(TicketStatus status) => status switch
        {
            TicketStatus.Acik => "Açık",
            TicketStatus.Cozuldu => "Çözüldü",
            TicketStatus.Kapandi => "Kapandı",
            _ => status.ToString()
        };
    }
}