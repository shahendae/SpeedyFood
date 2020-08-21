using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpeedyFood.Models;

namespace SpeedyFood.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category> Category { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    }
}
