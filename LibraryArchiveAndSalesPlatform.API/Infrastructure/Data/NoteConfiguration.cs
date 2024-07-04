using LibraryArchiveAndSalesPlatform.API.Domain.Enums;
using LibraryArchiveAndSalesPlatform.API.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryArchiveAndSalesPlatform.API.Infrastructure.Data
{
    public class NoteConfiguration : IEntityTypeConfiguration<Note>
    {
        public void Configure(EntityTypeBuilder<Note> builder)
        {
            builder.HasKey(n => n.Id);

            builder.HasOne(n => n.Book).WithMany(b => b.Notes).HasForeignKey(n => n.BookId).IsRequired();

            builder.Property(n => n.PrivacySetting)
               .HasDefaultValue(PrivacySetting.Private)
               .HasConversion(
                    n => n.ToString(),
                    dbStatus => dbStatus == null ? PrivacySetting.Private : (PrivacySetting)Enum.Parse(typeof(PrivacySetting), dbStatus))
               .HasDefaultValue(PrivacySetting.Private);
                
        }
    }
}
