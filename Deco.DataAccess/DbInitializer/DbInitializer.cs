using Deco.DataAccess.Data;
using Deco.Models;
using Deco.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deco.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public void Initialize()
        {
            try
            {
                //Migrate db to the lastest.
                if(_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }catch (Exception ex)
            {
                //Log exception here
            }

            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                //Create roles for the first time
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Hotel)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Dealer)).GetAwaiter().GetResult();

                //Create admin for the first time
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "amisszsiy@yahoo.com",
                    Email = "amisszsiy@yahoo.com",
                    FirstName = "Admin",
                    LastName = "Admin",
                    PhoneNumber = "123-456-7890",
                    StreetAddress = "Somewhere Rd.",
                    City = "Somewhere City",
                    Province = "Somewhere Province",
                    PostalCode = "12345"
                },
            "Admin123!").GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "amisszsiy@yahoo.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }

            return;
        }
    }
}
