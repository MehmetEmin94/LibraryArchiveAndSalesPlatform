using LibraryArchiveAndSalesPlatform.API.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryArchiveAndSalesPlatform.API.Infrastructure.Data
{
    public class ShelfConfigurations : IEntityTypeConfiguration<Shelf>
    {
        public void Configure(EntityTypeBuilder<Shelf> builder)
        {
            builder.HasKey(o => o.Id);

            builder.HasMany(b => b.Books).WithOne().HasForeignKey(n => n.ShelfId);
        }
    }
}
