using Microsoft.EntityFrameworkCore;
using Domain.Entites;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.Database.Configurations;

public class ItemImageConfiguration : IEntityTypeConfiguration<ItemImage>
{
    public void Configure(EntityTypeBuilder<ItemImage> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.ToTable("ItemImages");
        
        builder.HasOne(i => i.Item)
            .WithMany(i => i.Images)
            .OnDelete(DeleteBehavior.Cascade);
    }
}