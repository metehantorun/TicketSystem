using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.Models
{
    public class TicketReply
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Cevap metni zorunludur.")]
        [StringLength(2000, MinimumLength = 2, ErrorMessage = "Cevap 2-2000 karakter olmalıdır.")]
        [Display(Name = "Cevap")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Tarih")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsStaffReply { get; set; } = false;

        // FK
        public int TicketId { get; set; }
        [ForeignKey("TicketId")]
        public Ticket? Ticket { get; set; }

        public string AuthorId { get; set; } = string.Empty;
        [ForeignKey("AuthorId")]
        public ApplicationUser? Author { get; set; }
    }
}
