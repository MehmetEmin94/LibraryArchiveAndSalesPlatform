using LibraryArchiveAndSalesPlatform.API.Domain.Abstraction;

namespace LibraryArchiveAndSalesPlatform.API.Domain.Models
{
    public class Shelf : Entity<Guid>
    {
        public string Section { get; set; }
        public string Row { get; set; }
        public string Position { get; set; }
        public List<Book> Books { get; set; }
    }
}
