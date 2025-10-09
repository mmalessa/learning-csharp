using LearningCSharp.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace LearningCSharp.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
}