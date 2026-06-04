using System.ComponentModel.DataAnnotations;

namespace TicketSystem.Models.ViewModels
{
    // Login ViewModel
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email giriniz.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }

    // Register ViewModel
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ad Soyad 2-100 karakter olmalıdır.")]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email giriniz.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        [Display(Name = "Şifre Tekrar")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    // Create Ticket ViewModel
    public class CreateTicketViewModel
    {
        [Required(ErrorMessage = "Başlık zorunludur.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Başlık 5-200 karakter olmalıdır.")]
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Açıklama 10-2000 karakter olmalıdır.")]
        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Öncelik")]
        public TicketPriority Priority { get; set; } = TicketPriority.Orta;
    }

    // Ticket Detail ViewModel
    public class TicketDetailViewModel
    {
        public Ticket Ticket { get; set; } = null!;
        public List<TicketReply> Replies { get; set; } = new();
        public AddReplyViewModel NewReply { get; set; } = new();
        public bool IsStaff { get; set; }
    }

    // Add Reply ViewModel
    public class AddReplyViewModel
    {
        public int TicketId { get; set; }

        [Required(ErrorMessage = "Cevap metni zorunludur.")]
        [StringLength(2000, MinimumLength = 2, ErrorMessage = "Cevap 2-2000 karakter olmalıdır.")]
        [Display(Name = "Cevabınız")]
        public string Content { get; set; } = string.Empty;
    }

    // Ticket List ViewModel
    public class TicketListViewModel
    {
        public List<Ticket> Tickets { get; set; } = new();
        public TicketStatus? FilterStatus { get; set; }
        public TicketPriority? FilterPriority { get; set; }
        public string? SearchTerm { get; set; }
    }
}
