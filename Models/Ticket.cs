using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Başlık 5-200 karakter olmalıdır.")]
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Açıklama 10-2000 karakter olmalıdır.")]
        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Durum")]
        public TicketStatus Status { get; set; } = TicketStatus.Acik;

        [Display(Name = "Öncelik")]
        public TicketPriority Priority { get; set; } = TicketPriority.Orta;

        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }

        // FK - Müşteri (talebi oluşturan)
        public string CustomerId { get; set; } = string.Empty;
        [ForeignKey("CustomerId")]
        public ApplicationUser? Customer { get; set; }

        // FK - Destek Ekibi (talebi üstlenen)
        public string? SupportAgentId { get; set; }
        [ForeignKey("SupportAgentId")]
        public ApplicationUser? SupportAgent { get; set; }

        // Navigation
        public ICollection<TicketReply> Replies { get; set; } = new List<TicketReply>();
    }
}
