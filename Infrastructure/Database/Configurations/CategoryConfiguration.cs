using Microsoft.EntityFrameworkCore;
using Domain.Entites;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.Database.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        
        builder.HasKey(x => x.Id);
        
        builder.ToTable("Categories");
        
        builder.HasMany(i => i.Items)
            .WithOne(i => i.Category)
            .OnDelete(DeleteBehavior.Restrict);
    }
}