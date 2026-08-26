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
        public DbSet<WorkSpaceMembership> WorkSpaceMemberships { get; set; }
        public DbSet<WorkSpaceInvitation> WorkSpaceInvitations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkSpaceMembership>()
                .HasOne(m => m.User)
                .WithMany(u => u.WorkSpaceMemberships)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkSpaceMembership>()
                .HasOne(m => m.WorkSpace)
                .WithMany(w => w.Memberships)
                .HasForeignKey(m => m.WorkSpaceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
