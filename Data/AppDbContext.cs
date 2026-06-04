using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Models;

namespace TicketSystem.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketReply> TicketReplies { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Ticket -> Customer (SetNull on delete)
            builder.Entity<Ticket>()
                .HasOne(t => t.Customer)
                .WithMany(u => u.CreatedTickets)
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ticket -> SupportAgent (SetNull on delete)
            builder.Entity<Ticket>()
                .HasOne(t => t.SupportAgent)
                .WithMany(u => u.AssignedTickets)
                .HasForeignKey(t => t.SupportAgentId)
                .OnDelete(DeleteBehavior.SetNull);

            // TicketReply -> Author
            builder.Entity<TicketReply>()
                .HasOne(r => r.Author)
                .WithMany(u => u.Replies)
                .HasForeignKey(r => r.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // TicketReply -> Ticket
            builder.Entity<TicketReply>()
                .HasOne(r => r.Ticket)
                .WithMany(t => t.Replies)
                .HasForeignKey(r => r.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
