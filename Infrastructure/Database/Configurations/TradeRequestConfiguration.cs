using Microsoft.EntityFrameworkCore;
using Domain.Entites;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.Database.Configurations;

public class TradeRequestConfiguration : IEntityTypeConfiguration<TradeRequest>
{
    public void Configure(EntityTypeBuilder<TradeRequest> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.ToTable("TradeRequests");

        builder.HasOne(i => i.Item)
            .WithMany(r => r.Requests)
            .OnDelete(DeleteBehavior.Cascade);
    }
}