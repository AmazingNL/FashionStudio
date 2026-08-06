using FashionStudio.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace FashionStudio.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options)
            : base(options)
        {
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Fitting> Fittings { get; set; }
        public DbSet<MeasurementFiled> MeasurementFileds { get; set; }
        public DbSet<MeasurementSet> MeasurementSets { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderImage> OrderImages { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<WorkSpace> WorkSpaces { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Membership: WorkSpace 1 -> many Users
            modelBuilder.Entity<User>()
                .HasOne(u => u.WorkSpace)
                .WithMany(w => w.Users)
                .HasForeignKey(u => u.WorkSpaceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ownership: User 1 -> many WorkSpaces
            modelBuilder.Entity<WorkSpace>()
                .HasOne(w => w.Owner)
                .WithMany(u => u.OwnedWorkSpaces)
                .HasForeignKey(w => w.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

        
        }
    }
}
