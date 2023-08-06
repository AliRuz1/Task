using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tryy.Models;
namespace tryy.DATA
{
    public class AppDBContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<RoleModel> Roles { get; set; }

        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductModel>().HasData(
                new ProductModel { Id = 1, Name = "Селедка", CategoryId = 1, Description = "Селедека соленая", NoteGeneral = "Акция", NoteSpecial = "Пересоленая", Price = 10000 },
                new ProductModel { Id = 2, Name = "Тушенка", CategoryId = 1, Description = "Тушенка говяжая", NoteGeneral = "Вкусная", NoteSpecial = "Жилы", Price = 10000 }
            );

            modelBuilder.Entity<RoleModel>().HasData(
                new RoleModel { Id = 1, Name = "admin" },
                new RoleModel { Id = 2, Name = "user" },
                new RoleModel { Id = 3, Name = "special" }
            );

            // modelBuilder.Entity<CategoryModel>().HasData(
            //     new CategoryModel { Id = 1, Name = "Еда" },
            //     new CategoryModel { Id = 2, Name = "Напитки" }
            // );

            modelBuilder.Entity<UserModel>().HasData(
                new UserModel { Id = 1, Email = "user@gmail.com", Password = "1234" },
                new UserModel { Id = 2, Email = "special@gmail.com", Password = "1111" },
                new UserModel { Id = 3, Email = "admin@gmail.com", Password = "54321" }
            );
                        modelBuilder.Entity<ProductModel>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId);

                    modelBuilder.Entity<ProductModel>()
                .Property(p => p.NoteSpecial)
                .IsRequired(false);

            modelBuilder.Entity<UserModel>()
                .HasMany(u => u.Roles)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    ur => ur.HasOne<RoleModel>().WithMany().HasForeignKey("RoleId"),
                    ur => ur.HasOne<UserModel>().WithMany().HasForeignKey("UserId"),
                    ur =>
                    {
                        ur.HasKey("UserId", "RoleId");
                        ur.HasData(
                            new { UserId = 1, RoleId = 2 },
                            new { UserId = 2, RoleId = 3 },
                            new { UserId = 3, RoleId = 1 }
                        );
                    }
                );
        }
    }
}
