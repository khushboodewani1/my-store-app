using Microsoft.EntityFrameworkCore;
using MyStore.Model;
using System.Collections.Generic;
using System.Reflection.Emit;
using MyStore.Model.Helpers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace MyStore.Data
{
    /// <summary>
    /// The ApplicationDbContext class is a custom database context class derived from DbContext, 
    /// responsible for interacting with the underlying database using Entity Framework.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<CategoryMaster> Categories => Set<CategoryMaster>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Snake_case naming convention
            var rootEntities = modelBuilder.Model.GetEntityTypes()
                            .Where(e => !e.IsOwned());
            foreach (var entity in rootEntities)
            {
                entity.SetTableName(entity.GetTableName()!.ToSnakeCase());
            }

          
            foreach (var prop in modelBuilder.Model.GetEntityTypes()
                                         .SelectMany(t => t.GetProperties()))
            {
                prop.SetColumnName(prop.Name.ToSnakeCase());
            }
            modelBuilder.Entity<UserRole>()
         .HasKey(ur => new { ur.UserId, ur.RoleId });
            modelBuilder.Entity<UserRole>()
              .HasOne(ur => ur.User)
              .WithMany(u => u.UserRoles)
              .HasForeignKey(ur => ur.UserId);
            modelBuilder.Entity<UserRole>()
              .HasOne(ur => ur.Role)
              .WithMany(r => r.UserRoles)
              .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<Product>(builder =>
            {
                builder.OwnsOne(p => p.Price, nav =>
                {
                    // Define the foreign key for the owned entity (Price) 
                    nav.WithOwner().HasForeignKey("product_id");

                    // Configure the Amount property with the name "price_amount" and decimal type
                    nav.Property(p => p.Amount)
                       .HasColumnName("price_amount")
                      .HasColumnType("decimal(18, 2)");

                    // Configure the Currency property with the name "price_currency"
                    nav.Property(p => p.Currency)
                       .HasColumnName("price_currency");
                       
                });
            });
           
            base.OnModelCreating(modelBuilder);
        }
    }
}
