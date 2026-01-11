using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HotelBooking.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Persistence.Db;

public sealed class HotelBookingDbContext : DbContext
{
    public HotelBookingDbContext(DbContextOptions<HotelBookingDbContext> options) : base(options) { }

    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<RoomAvailability> Availability => Set<RoomAvailability>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Hotel
        modelBuilder.Entity<Hotel>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Hotel>()
            .Property(x => x.Id)
            .HasMaxLength(64);

        modelBuilder.Entity<Hotel>()
            .Property(x => x.Destination)
            .HasMaxLength(128);

        // Availability: index for fast search
        modelBuilder.Entity<RoomAvailability>()
            .HasIndex(x => new { x.HotelId, x.Date, x.RoomType })
            .IsUnique(false);

        modelBuilder.Entity<RoomAvailability>()
            .Property(x => x.RoomType)
            .HasMaxLength(64);

        // Reservation
        modelBuilder.Entity<Reservation>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Reservation>()
            .Property(x => x.Id)
            .HasMaxLength(64);

        // Notification
        modelBuilder.Entity<Notification>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Notification>()
            .Property(x => x.Id)
            .HasMaxLength(64);
    }
}

