using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.ToTable("Users");
        builder.Property(u => u.Name)
            .IsRequired();

        builder.HasMany(i => i.Items)
            .WithOne(i => i.Owner)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(t =>t.TradeRequests)
            .WithOne(s => s.Receiver)
            .OnDelete(DeleteBehavior.Restrict);
        
    }
}