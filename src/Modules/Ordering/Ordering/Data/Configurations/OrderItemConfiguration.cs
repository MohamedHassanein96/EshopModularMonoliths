using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Ordering.Data.Configurations
{
    internal class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.ProductId).IsRequired();

            builder.Property(oi => oi.Quantity).IsRequired();

            builder.Property(oi => oi.Price).IsRequired();
        }
    }
}
