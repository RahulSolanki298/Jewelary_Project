using Common;
using ControlPanel.Services.IServices;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ControlPanel.Services
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDBContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDBContext db, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task Initalize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Any())
                {
                    await _db.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Migration Error: {ex.Message}");
            }

            await SeedRolesAsync();

            await SeedAdminUserAsync();

            await SeedCategoriesAsync();

            //await SeedSubCategoriesAsync();

            await SeedProductPropertiesAsync();
        }

        private async Task SeedRolesAsync()
        {
            var roles = new List<string>
            {
                SD.Admin,
                SD.Customer,
                SD.Employee,
                SD.Supplier,
                SD.BusinessAccount
            };

            foreach (var role in roles)
            {
                if (!await _db.Roles.AnyAsync(x => x.Name == role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private async Task SeedAdminUserAsync()
        {
            if (!await _db.ApplicationUser.AnyAsync())
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "Vivek",
                    LastName = "Godhani",
                    Gender = "Male",
                    TextPassword = "Admin123*"
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin123*");
                if (result.Succeeded)
                {
                    var user = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Email == "admin@gmail.com");
                    if (user != null)
                    {
                        await _userManager.AddToRoleAsync(user, SD.Admin);
                    }
                }
                else
                {
                    // Log error if user creation fails
                    Console.WriteLine("Admin user creation failed.");
                }
            }
        }

        private async Task SeedCategoriesAsync()
        {
            if (!await _db.Category.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Rings", ProductType = "Diamond",IsActivated=true },
                    new Category { Name = "Earrings", ProductType = "Diamond",IsActivated=true },
                    new Category { Name = "Nackleces", ProductType = "Diamond",IsActivated=true },
                    new Category { Name = "Pendants", ProductType = "Diamond",IsActivated=true },
                    new Category { Name = "Bracelets", ProductType = "Diamond",IsActivated=true },
                    new Category { Name = "Cufflinks", ProductType = "Diamond",IsActivated=true },
                    new Category { Name = "Brooch", ProductType = "Diamond" ,IsActivated=true},
                    new Category { Name = "Bangle", ProductType = "Diamond" ,IsActivated=true}
                };
                await _db.Category.AddRangeAsync(categories);
                await _db.SaveChangesAsync();
            }
        }

        //private async Task SeedSubCategoriesAsync()
        //{
        //    if (!await _db.SubCategory.AnyAsync())
        //    {
        //        var ringCategory = await _db.Category.Where(x => x.Name == "Rings").FirstOrDefaultAsync();

        //        if (ringCategory != null)
        //        {
        //            var subCategories = new List<SubCategory>
        //            {
        //                new SubCategory { Name = "Solitaire Rings", CategoryId = ringCategory.Id },
        //                new SubCategory { Name = "Earrings", CategoryId = ringCategory.Id },
        //                new SubCategory { Name = "Nackleces", CategoryId = ringCategory.Id },
        //                new SubCategory { Name = "Pendants", CategoryId = ringCategory.Id },
        //                new SubCategory { Name = "Bracelets", CategoryId = ringCategory.Id },
        //                new SubCategory { Name = "Cufflinks", CategoryId = ringCategory.Id },
        //                new SubCategory { Name = "Brooch", CategoryId = ringCategory.Id },
        //                new SubCategory { Name = "Bangle", CategoryId = ringCategory.Id }
        //            };
        //            await _db.SubCategory.AddRangeAsync(subCategories);
        //            await _db.SaveChangesAsync();
        //        }
        //    }
        //}

        private async Task SeedProductPropertiesAsync()
        {
            if (!await _db.ProductProperty.AnyAsync())
            {
                var ringCategory = await _db.Category.Where(x => x.Name == "Rings").FirstOrDefaultAsync();
                try
                {
                    if (ringCategory != null)
                    {
                        var productProperties = new List<ProductProperty>
                    {
                        new ProductProperty { Name = "Metal", Description="-" },
                        new ProductProperty { Name = "Clarity" },
                        new ProductProperty { Name = "Shape" },
                        new ProductProperty { Name = "Carat"},
                    };
                        await _db.ProductProperty.AddRangeAsync(productProperties);
                        await _db.SaveChangesAsync();
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
