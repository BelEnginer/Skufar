using Microsoft.EntityFrameworkCore;
using Domain.Entites;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.Database.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.ToTable("Items");
        
        builder.HasOne(u => u.Owner)
            .WithMany(u => u.Items)
            .OnDelete(DeleteBehavior.Cascade);
        
    }
}