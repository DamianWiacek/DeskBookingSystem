using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskBookingSystem.Entities
{
    public class BookingSystemDbContext : DbContext
    {
        private string _connectionString = "Server=DESKTOP-2UA5DVQ;Database=DeskBookingSystemDb;Trusted_Connection=True;";

        public DbSet<Desk> Desks { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Role>()
            .HasData(
            new Role() { Id = 1, Name = "Employee" },
            new Role() { Id = 2, Name = "Administrator" });

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
