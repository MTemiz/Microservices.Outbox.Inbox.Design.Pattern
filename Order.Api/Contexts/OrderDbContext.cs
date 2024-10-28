using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;

namespace Order.Api.Contexts;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Models.Order> Orders { get; set; }
    public DbSet<Models.OrderItem> OrderItems { get; set; }
    public DbSet<Models.OrderOutbox> OrderOutboxes { get; set; }
    
}