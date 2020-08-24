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
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(IUnitOfWork unitOfWork, 
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }
        public async void Initialize()
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
    }
}
