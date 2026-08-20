using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class CarListDbContext : IdentityDbContext
{
    public CarListDbContext(DbContextOptions<CarListDbContext> options) : base(options)
    {

    }

    public DbSet<Car> Cars { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Car>().HasData(
            new Car
            {
                Id = 1,
                Make = "Honda",
                Model = "Fit",
                Vin="ABC"
            },
            new Car
            {
                Id = 2,
                Make = "Honda",
                Model = "Civic",
                Vin = "ABC2"
            },
            new Car
            {
                Id = 3,
                Make = "Honda",
                Model = "Stream",
                Vin = "ABC1"
            },
            new Car
            {
                Id = 4,
                Make = "Nissan",
                Model = "Note",
                Vin = "ABC4"
            },
            new Car
            {
                Id = 5,
                Make = "Nissan",
                Model = "Atlas",
                Vin = "ABC5"
            },
            new Car
            {
                Id = 6,
                Make = "Nissan",
                Model = "Dualis",
                Vin = "ABC6"
            },
            new Car
            {
                Id = 7,
                Make = "Nissan",
                Model = "Murano",
                Vin = "ABC7"
            },
            new Car
            {
                Id = 8,
                Make = "Audi",
                Model = "A5",
                Vin = "ABC8"
            },
            new Car
            {
                Id = 9,
                Make = "BMW",
                Model = "M3",
                Vin = "ABC9"
            },
            new Car
            {
                Id = 10,
                Make = "Jaguar",
                Model = "F-Pace",
                Vin = "ABC10"
            }
        );

        modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "d1b5952a-2162-46c7-b29e-1a2a68922c14",
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR",
                    ConcurrencyStamp = "1"
                },
                new IdentityRole
                {
                    Id = "42358d3e-3c22-45e1-be81-6caa7ba865ef",
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = "2"
                }
            );
        //AQAAAAIAAYagAAAAEMHjJaG7J0Z3qH8qYqQqO8/FqQz8qKvVqv3bqk7FqQz8qKvVqv3bqk7FqQz8qKvVqw==
        // Password: P@ssword1 (hashed values are hardcoded to avoid dynamic model changes)
        modelBuilder.Entity<IdentityUser>().HasData(
                new IdentityUser
                {
                    Id = "408aa945-3d84-4421-8342-7269ec64d949",
                    Email = "admin@localhost.com",
                    NormalizedEmail = "ADMIN@LOCALHOST.COM",
                    NormalizedUserName = "ADMIN@LOCALHOST.COM",
                    UserName = "admin@localhost.com",
                    PasswordHash = "AQAAAAIAAYagAAAAEMHjJaG7J0Z3qH8qYqQqO8/FqQz8qKvVqv3bqk7FqQz8qKvVqv3bqk7FqQz8qKvVqw==",
                    EmailConfirmed = true,
                    ConcurrencyStamp = "c8554266-b401-4519-9aeb-ff6a64abd724",
                    SecurityStamp = "c8554266-b401-4519-9aeb-ff6a64abd725"
                },
                new IdentityUser
                {
                    Id = "3f4631bd-f907-4409-b416-ba356312e659",
                    Email = "user@localhost.com",
                    NormalizedEmail = "USER@LOCALHOST.COM",
                    NormalizedUserName = "USER@LOCALHOST.COM",
                    UserName = "user@localhost.com",
                    PasswordHash = "AQAAAAIAAYagAAAAEMHjJaG7J0Z3qH8qYqQqO8/FqQz8qKvVqv3bqk7FqQz8qKvVqv3bqk7FqQz8qKvVqw==",
                    EmailConfirmed = true,
                    ConcurrencyStamp = "c8554266-b401-4519-9aeb-ff6a64abd726",
                    SecurityStamp = "c8554266-b401-4519-9aeb-ff6a64abd727"
                }
            );

        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                 new IdentityUserRole<string>
                 {
                     RoleId = "d1b5952a-2162-46c7-b29e-1a2a68922c14",
                     UserId = "408aa945-3d84-4421-8342-7269ec64d949",
                 },
                new IdentityUserRole<string>
                {
                    RoleId = "42358d3e-3c22-45e1-be81-6caa7ba865ef",
                    UserId = "3f4631bd-f907-4409-b416-ba356312e659",
                }
            );
    }
}