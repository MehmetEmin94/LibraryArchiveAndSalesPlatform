using LibraryArchiveAndSalesPlatform.API.Domain.Abstraction;
using LibraryArchiveAndSalesPlatform.API.Domain.ValueObjects;

namespace LibraryArchiveAndSalesPlatform.API.Domain.Models
{
    public class Book : Entity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ShelfId { get; set; }
        public List<Note> Notes { get; set; } = new();
        public FileContent Image { get; set; }
    }
}
