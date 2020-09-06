using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpeedyFood.Models;
using SpeedyFood.Repository;
using SpeedyFood.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer( 
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }
        public async Task Initialize()
        {
            //if there is no database, check if there is migrations
            if (_db.Database.GetPendingMigrations().Count() > 0)
            {
                _db.Database.Migrate();
            }

            //Create Roles
            if (_db.Roles.Any(r => r.Name == StaticDetails.ManagerUser)) return;

            _roleManager.CreateAsync(new IdentityRole(StaticDetails.ManagerUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticDetails.KitchenUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticDetails.CustomerEndUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticDetails.FrontDeskUser)).GetAwaiter().GetResult();

            //Add Default Admin User
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                PhoneNumber = "012345678923",
                Name = "Shahenda Elsayed",
                EmailConfirmed = true
            }, "Admin@123").GetAwaiter().GetResult();

            IdentityUser user = _db.Users.FirstOrDefault(m => m.Email == "admin@gmail.com");
            await _userManager.AddToRoleAsync(user, StaticDetails.ManagerUser);

        }

        public void SeedData()
        {
            if (!_db.Category.Any())
            {
                var defaultCategories = new Category[]
                {
                    new Category(){Name = "Apetizer"},
                    new Category(){Name = "Main Course"},
                    new Category(){Name = "Desert"},
                    new Category(){Name = "Drink"}
                };
                _db.Category.AddRange(defaultCategories);
                _db.SaveChanges();
            }

            if (!_db.SubCategories.Any())
            {
                var defaultSubCategories = new SubCategory[]
                {
                    new SubCategory(){Name = "Veg", CategoryId = 1},
                    new SubCategory(){Name = "Not Veg", CategoryId = 1},
                    new SubCategory(){Name = "Veg", CategoryId = 2},
                    new SubCategory(){Name = "Not Veg", CategoryId = 2},
                    new SubCategory(){Name = "Sugar Free", CategoryId = 3},
                    new SubCategory(){Name = "Sweet", CategoryId = 3},
                    new SubCategory(){Name = "Alcoholic", CategoryId = 4},
                    new SubCategory(){Name = "Fresh Juice", CategoryId = 4}
                };

                _db.SubCategories.AddRange(defaultSubCategories);
                _db.SaveChanges();
            }

            if (!_db.MenuItems.Any())
            {
                var defaultMenuItems = new MenuItem[]
                {
                    new MenuItem(){Name = "Samosa", Price = 50, Image ="\\images\\1.jpg", CategoryId = 1, SubCategoryId = 1, Description="It is a Vg Appetizer There are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form, by injected humour, or randomised words which don't look even slightly believable. If you are going to use a passage of Lorem Ipsum."},
                    new MenuItem(){Name = "Spring Rolls", Price = 60, Image =@"\images\2.jpg", CategoryId = 1, SubCategoryId = 2, Description="It is a Not Veg Appetizer There are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form, by injected humour, or randomised words which don't look even slightly believable. If you are going to use a passage of Lorem Ipsum."},
                    new MenuItem(){Name = "Chicken", Price = 120, Image =@"\images\3.jpg", CategoryId = 2, SubCategoryId = 4, Description="It is a Not Veg Main Courser There are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form, by injected humour, or randomised words which don't look even slightly believable. If you are going to use a passage of Lorem Ipsum."},
                    new MenuItem(){Name = "Kofta", Price = 150, Image =@"\images\4.jpg", CategoryId = 2, SubCategoryId = 4, Description="It is a Not Veg Main Courser There are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form, by injected humour, or randomised words which don't look even slightly believable. If you are going to use a passage of Lorem Ipsum."},
                    new MenuItem(){Name = "Konafa", Price = 40, Image =@"\images\5.jpg", CategoryId = 3, SubCategoryId = 6, Description="It is a Sweet Desert There are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form, by injected humour, or randomised words which don't look even slightly believable. If you are going to use a passage of Lorem Ipsum."},
                    new MenuItem(){Name = "Biscuits", Price = 30, Image =@"\images\6.jpeg", CategoryId = 3, SubCategoryId = 5, Description="It is a Sugar Free There are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form, by injected humour, or randomised words which don't look even slightly believable. If you are going to use a passage of Lorem Ipsum."},
                    new MenuItem(){Name = "Mango", Price = 55, Image =@"\images\7.jpg", CategoryId = 4, SubCategoryId = 8, Description="It is a drink There are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form, by injected humour, or randomised words which don't look even slightly believable. If you are going to use a passage of Lorem Ipsum."},
                    new MenuItem(){Name = "ButterMilk", Price = 45, Image =@"\images\8.jpg", CategoryId = 4, SubCategoryId = 8, Description="It is a drink There are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form, by injected humour, or randomised words which don't look even slightly believable. If you are going to use a passage of Lorem Ipsum."},
                    new MenuItem(){Name = "Rice", Price = 70, Image =@"\images\9.jpg", CategoryId = 2, SubCategoryId = 3, Description="It is a Veg Main Courser There are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form, by injected humour, or randomised words which don't look even slightly believable. If you are going to use a passage of Lorem Ipsum."},
                };

                _db.MenuItems.AddRange(defaultMenuItems);
                _db.SaveChanges();
            }
            
        }
    }
}
