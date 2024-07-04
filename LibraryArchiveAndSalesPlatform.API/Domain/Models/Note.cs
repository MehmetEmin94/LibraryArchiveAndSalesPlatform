using LibraryArchiveAndSalesPlatform.API.Domain.Abstraction;
using LibraryArchiveAndSalesPlatform.API.Domain.Enums;

namespace LibraryArchiveAndSalesPlatform.API.Domain.Models
{
    public class Note : Entity<Guid>
    {
        public Guid BookId { get; set; }
        public Book Book { get; set; } 
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsPrivate { get; set; } = false;
        public PrivacySetting PrivacySetting { get; set; } = PrivacySetting.Private;
    }
}
