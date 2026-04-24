
namespace HotelBooking.Data;

using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;



public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Promotion> Promotions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Unique constraint on email
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        // Seed sample data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder builder)
    {
        builder.Entity<Hotel>().HasData(
            new Hotel { Id = 1, Name = "Grand Plaza", Location = "New York", Description = "Luxury hotel in downtown", Amenities = "Pool,WiFi,Gym,Spa", AverageRating = 4.5 },
            new Hotel { Id = 2, Name = "Ocean View", Location = "Miami", Description = "Beachfront hotel", Amenities = "Beach Access,WiFi,Restaurant,Bar", AverageRating = 4.2 },
            new Hotel { Id = 3, Name = "Mountain Retreat", Location = "Denver", Description = "Cozy mountain lodge", Amenities = "Hiking,WiFi,Fireplace", AverageRating = 4.7 }
        );

        builder.Entity<Room>().HasData(
            new Room { Id = 1, RoomNumber = "101", Type = "Deluxe", PricePerNight = 150, MaxOccupancy = 2, HotelId = 1 },
            new Room { Id = 2, RoomNumber = "102", Type = "Suite", PricePerNight = 250, MaxOccupancy = 4, HotelId = 1 },
            new Room { Id = 3, RoomNumber = "103", Type = "Standard", PricePerNight = 100, MaxOccupancy = 2, HotelId = 1 },
            new Room { Id = 4, RoomNumber = "201", Type = "Standard", PricePerNight = 120, MaxOccupancy = 2, HotelId = 2 },
            new Room { Id = 5, RoomNumber = "202", Type = "Deluxe", PricePerNight = 180, MaxOccupancy = 3, HotelId = 2 },
            new Room { Id = 6, RoomNumber = "301", Type = "Suite", PricePerNight = 220, MaxOccupancy = 4, HotelId = 3 }
        );

        builder.Entity<Promotion>().HasData(
            new Promotion { Id = 1, Code = "WELCOME10", DiscountType = "Percentage", DiscountValue = 10, ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddMonths(3), IsActive = true },
            new Promotion { Id = 2, Code = "SUMMER50", DiscountType = "Fixed", DiscountValue = 50, ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddMonths(1), IsActive = true }
        );
    }
}

