using Microsoft.EntityFrameworkCore;
using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.Database.Configurations;

public class RewievConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.ToTable("Rewievs");

        builder.HasOne(r =>r.Receiver)
            .WithMany(r => r.Reviews)
            .OnDelete(DeleteBehavior.Cascade);
    }
}