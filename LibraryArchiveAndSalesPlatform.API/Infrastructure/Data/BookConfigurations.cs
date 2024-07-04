using LibraryArchiveAndSalesPlatform.API.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryArchiveAndSalesPlatform.API.Infrastructure.Data
{
    public class BookConfigurations : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasKey(o => o.Id);

            builder.HasMany(b => b.Notes).WithOne().HasForeignKey(n => n.BookId);

            builder.ComplexProperty(
                b => b.Image, nameBuilder =>
                {
                    nameBuilder.Property(n => n.Name);
                    nameBuilder.Property(n => n.ContentType);
                    nameBuilder.Property(n => n.Content);
                });
        }
    }
}
