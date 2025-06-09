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

            await SeedDiamondPropertiesAsync();

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

        private async Task SeedProductPropertiesAsync()
        {
            if (!await _db.ProductProperty.AnyAsync())
            {
                try
                {
                    // Main Property for Jewellery
                    var productProperties = new List<ProductProperty>
                        {
                            new ProductProperty { Name = "Metal", Description="-",IsActive=true },
                            new ProductProperty { Name = "Clarity",Description="-",IsActive=true },
                            new ProductProperty { Name = "Shape",Description="-",IsActive=true },
                            new ProductProperty { Name = "Carat",Description="-",IsActive=true},
                            new ProductProperty { Name = "Center-Carat",Description="-",IsActive=true},
                            new ProductProperty { Name = "Grades",Description="-",IsActive=true},
                            new ProductProperty { Name = "Karat",Description="-",IsActive=true},
                            new ProductProperty { Name = "GoldWeight",Description="-",IsActive=true},
                        };
                    await _db.ProductProperty.AddRangeAsync(productProperties);
                    await _db.SaveChangesAsync();

                    // Add Metals
                    var metal = await _db.ProductProperty.Where(x => x.Name == SD.Metal).FirstOrDefaultAsync();
                    var metalDTs = new List<ProductProperty>
                    {
                        new ProductProperty{ Name="White",ParentId=metal.Id,SymbolName="white",DisplayOrder=1,IsActive=true,Synonyms="W,white,WG,White"},
                        new ProductProperty{ Name="Yellow",ParentId=metal.Id,SymbolName="yellow",DisplayOrder=1,IsActive=true,Synonyms="Y,Yellow,yellow,YG"},
                        new ProductProperty{ Name="Rose",ParentId=metal.Id,SymbolName="rose",DisplayOrder=1,IsActive=true,Synonyms="rose,RG,Rose"},
                        new ProductProperty{ Name="E-F",ParentId=metal.Id,SymbolName="E-F",DisplayOrder=1,IsActive=true,Synonyms="E-F"},
                    };
                    await _db.ProductProperty.AddRangeAsync(metalDTs);
                    await _db.SaveChangesAsync();


                    // Add Clarity
                    //var clarity = await _db.ProductProperty.Where(x => x.Name == SD.Clarity).FirstOrDefaultAsync();
                    //var ClarityDTs = new List<ProductProperty>
                    //{
                    //    new ProductProperty{ Name="White",ParentId=clarity.Id,SymbolName="white",DisplayOrder=1,IsActive=true,Synonyms="W,white,WG,White"},
                    //    new ProductProperty{ Name="Yellow",ParentId=clarity.Id,SymbolName="yellow",DisplayOrder=1,IsActive=true,Synonyms="Y,Yellow,yellow,YG"},
                    //    new ProductProperty{ Name="Rose",ParentId=clarity.Id,SymbolName="rose",DisplayOrder=1,IsActive=true,Synonyms="rose,RG,Rose"},
                    //    new ProductProperty{ Name="E-F",ParentId=clarity.Id,SymbolName="E-F",DisplayOrder=1,IsActive=true,Synonyms="E-F"},
                    //};
                    //await _db.ProductProperty.AddRangeAsync(ClarityDTs);
                    //await _db.SaveChangesAsync();

                    // Add Metals
                    var shape = await _db.ProductProperty.Where(x => x.Name == SD.Shape).FirstOrDefaultAsync();
                    var shapeDTs = new List<ProductProperty>
                    {
                        new ProductProperty{ Name="ROUND",ParentId=shape.Id,SymbolName="round",DisplayOrder=1,IsActive=true,Synonyms="round",IconPath="/assets/img/diamond-svg/ROUND.svg"},
                        new ProductProperty{ Name="PRINCESS",ParentId=shape.Id,SymbolName="princess",DisplayOrder=1,IsActive=true,Synonyms="princess",IconPath="/assets/img/diamond-svg/PRINCESS.svg"},
                        new ProductProperty{ Name="MARQUISE",ParentId=shape.Id,SymbolName="marquise",DisplayOrder=1,IsActive=true,Synonyms="marquise",IconPath="/assets/img/diamond-svg/MARQUISE.svg"},
                        new ProductProperty{ Name="PEAR",ParentId=shape.Id,SymbolName="pear",DisplayOrder=1,IsActive=true,Synonyms="pear",IconPath="/assets/img/diamond-svg/PEAR.svg"},
                        new ProductProperty{ Name="HEART",ParentId=shape.Id,SymbolName="heart",DisplayOrder=1,IsActive=true,Synonyms="heart",IconPath="/assets/img/diamond-svg/HEART.svg"},
                        new ProductProperty{ Name="OVAL",ParentId=shape.Id,SymbolName="oval",DisplayOrder=1,IsActive=true,Synonyms="oval",IconPath="/assets/img/diamond-svg/OVAL.svg"},
                        new ProductProperty{ Name="CUSHION",ParentId=shape.Id,SymbolName="cushion",DisplayOrder=1,IsActive=true,Synonyms="cushion",IconPath="/assets/img/diamond-svg/CUSHION.svg"},
                        new ProductProperty{ Name="EMERALD",ParentId=shape.Id,SymbolName="emerald",DisplayOrder=1,IsActive=true,Synonyms="emerald",IconPath="/assets/img/diamond-svg/Emerald.svg"},
                        new ProductProperty{ Name="RADIANT",ParentId=shape.Id,SymbolName="radiant",DisplayOrder=1,IsActive=true,Synonyms="radiant",IconPath="/assets/img/diamond-svg/RADIANT.svg"},
                        new ProductProperty{ Name="SQ EMERALD",ParentId=shape.Id,SymbolName="sq emerald",DisplayOrder=1,IsActive=true,Synonyms="sq emerald"},
                    };
                    await _db.ProductProperty.AddRangeAsync(shapeDTs);
                    await _db.SaveChangesAsync();

                    var centalcarat = await _db.ProductProperty.Where(x => x.Name == SD.CaratSize).FirstOrDefaultAsync();
                    var centalcaratDTs = new List<ProductProperty>
                    {
                        new ProductProperty{ Name="0.05",ParentId=shape.Id,DisplayOrder=1,IsActive=true},
                        new ProductProperty{ Name="0.12",ParentId=shape.Id,DisplayOrder=1,IsActive=true},
                        new ProductProperty{ Name="0.23",ParentId=shape.Id,DisplayOrder=1,IsActive=true},
                        new ProductProperty{ Name="0.55",ParentId=shape.Id,DisplayOrder=1,IsActive=true},
                        new ProductProperty{ Name="1",ParentId=shape.Id,DisplayOrder=1,IsActive=true},
                        new ProductProperty{ Name="2",ParentId=shape.Id,DisplayOrder=1,IsActive=true},
                        new ProductProperty{ Name="3",ParentId=shape.Id,DisplayOrder=1,IsActive=true},
                        new ProductProperty{ Name="4",ParentId=shape.Id,DisplayOrder=1,IsActive=true},
                        new ProductProperty{ Name="5",ParentId=shape.Id,DisplayOrder=1,IsActive=true},
                        new ProductProperty{ Name="6",ParentId=shape.Id,DisplayOrder=1,IsActive=true},
                    };
                    await _db.ProductProperty.AddRangeAsync(centalcaratDTs);
                    await _db.SaveChangesAsync();

                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        private async Task SeedDiamondPropertiesAsync()
        {
            if (!await _db.DiamondProperties.AnyAsync())
            {
                try
                {
                    // Main Property for Jewellery
                    var diamondProperties = new List<DiamondProperty>
                        {
                            new DiamondProperty { Name = "TYPE", Description="-",IsActivated=true },
                            new DiamondProperty { Name = "Lab", Description="-",IsActivated=true },
                            new DiamondProperty { Name = "Color", Description="-",IsActivated=true },
                            new DiamondProperty { Name = "Clarity",Description="-",IsActivated=true },
                            new DiamondProperty { Name = "Shape",Description="-",IsActivated=true },
                            new DiamondProperty { Name = "Carat",Description="-",IsActivated=true},
                            new DiamondProperty { Name = "Cut",Description="-",IsActivated=true},
                            new DiamondProperty { Name = "Polish",Description="-",IsActivated=true},
                            new DiamondProperty { Name = "Symmetry",Description="-",IsActivated=true},
                            new DiamondProperty { Name = "Fluor",Description="-",IsActivated=true},
                            new DiamondProperty { Name = "Ratio",Description="-",IsActivated=true},
                            new DiamondProperty { Name = "CaratSize",Description="-",IsActivated=true},
                        };
                    await _db.DiamondProperties.AddRangeAsync(diamondProperties);
                    await _db.SaveChangesAsync();

                    // Add Metals
                    var color = await _db.DiamondProperties.Where(x => x.Name == SD.Color).FirstOrDefaultAsync();
                    var colorDTs = new List<DiamondProperty>
                    {
                        new DiamondProperty{ Name="D",ParentId=color.Id,DispOrder=1,IsActivated=true},
                        new DiamondProperty{ Name="E",ParentId=color.Id,DispOrder=2,IsActivated=true},
                        new DiamondProperty{ Name="F",ParentId=color.Id,DispOrder=3,IsActivated=true},
                        new DiamondProperty{ Name="G",ParentId=color.Id,DispOrder=4,IsActivated=true},
                    };
                    await _db.DiamondProperties.AddRangeAsync(colorDTs);
                    await _db.SaveChangesAsync();


                    // Add Clarity
                    //var clarity = await _db.ProductProperty.Where(x => x.Name == SD.Clarity).FirstOrDefaultAsync();
                    //var ClarityDTs = new List<ProductProperty>
                    //{
                    //    new ProductProperty{ Name="White",ParentId=clarity.Id,SymbolName="white",DisplayOrder=1,IsActive=true,Synonyms="W,white,WG,White"},
                    //    new ProductProperty{ Name="Yellow",ParentId=clarity.Id,SymbolName="yellow",DisplayOrder=1,IsActive=true,Synonyms="Y,Yellow,yellow,YG"},
                    //    new ProductProperty{ Name="Rose",ParentId=clarity.Id,SymbolName="rose",DisplayOrder=1,IsActive=true,Synonyms="rose,RG,Rose"},
                    //    new ProductProperty{ Name="E-F",ParentId=clarity.Id,SymbolName="E-F",DisplayOrder=1,IsActive=true,Synonyms="E-F"},
                    //};
                    //await _db.ProductProperty.AddRangeAsync(ClarityDTs);
                    //await _db.SaveChangesAsync();

                    // Add Metals
                    var shape = await _db.ProductProperty.Where(x => x.Name == SD.Shape).FirstOrDefaultAsync();
                    var shapeDTs = new List<ProductProperty>
                    {
                        new ProductProperty{ Name="ROUND",ParentId=shape.Id,SymbolName="round",DisplayOrder=1,IsActive=true,Synonyms="round",IconPath="/assets/img/diamond-svg/ROUND.svg"},
                        new ProductProperty{ Name="PRINCESS",ParentId=shape.Id,SymbolName="princess",DisplayOrder=1,IsActive=true,Synonyms="princess",IconPath="/assets/img/diamond-svg/PRINCESS.svg"},
                        new ProductProperty{ Name="MARQUISE",ParentId=shape.Id,SymbolName="marquise",DisplayOrder=1,IsActive=true,Synonyms="marquise",IconPath="/assets/img/diamond-svg/MARQUISE.svg"},
                        new ProductProperty{ Name="PEAR",ParentId=shape.Id,SymbolName="pear",DisplayOrder=1,IsActive=true,Synonyms="pear",IconPath="/assets/img/diamond-svg/PEAR.svg"},
                        new ProductProperty{ Name="HEART",ParentId=shape.Id,SymbolName="heart",DisplayOrder=1,IsActive=true,Synonyms="heart",IconPath="/assets/img/diamond-svg/HEART.svg"},
                        new ProductProperty{ Name="OVAL",ParentId=shape.Id,SymbolName="oval",DisplayOrder=1,IsActive=true,Synonyms="oval",IconPath="/assets/img/diamond-svg/OVAL.svg"},
                        new ProductProperty{ Name="CUSHION",ParentId=shape.Id,SymbolName="cushion",DisplayOrder=1,IsActive=true,Synonyms="cushion",IconPath="/assets/img/diamond-svg/CUSHION.svg"},
                        new ProductProperty{ Name="EMERALD",ParentId=shape.Id,SymbolName="emerald",DisplayOrder=1,IsActive=true,Synonyms="emerald",IconPath="/assets/img/diamond-svg/Emerald.svg"},
                        new ProductProperty{ Name="RADIANT",ParentId=shape.Id,SymbolName="radiant",DisplayOrder=1,IsActive=true,Synonyms="radiant",IconPath="/assets/img/diamond-svg/RADIANT.svg"},
                        new ProductProperty{ Name="SQ EMERALD",ParentId=shape.Id,SymbolName="sq emerald",DisplayOrder=1,IsActive=true,Synonyms="sq emerald"},
                    };
                    await _db.ProductProperty.AddRangeAsync(shapeDTs);
                    await _db.SaveChangesAsync();

                    var centalcarat = await _db.ProductProperty.Where(x => x.Name == SD.CaratSize).FirstOrDefaultAsync();
                    var centalcaratDTs = new List<ProductProperty>
                    {
                        new ProductProperty{ Name="0.05",ParentId=shape.Id,DisplayOrder=1,IsActive=true},
                        new ProductProperty{ Name="0.12",ParentId=shape.Id,DisplayOrder=1,IsActive=true},
                        new ProductProperty{ Name="0.23",ParentId=shape.Id,DisplayOrder=1,IsActive=true},
                        new ProductProperty{ Name="0.55",ParentId=shape.Id,DisplayOrder=1,IsActive=true},
                        new ProductProperty{ Name="1",ParentId=shape.Id,DisplayOrder=1,IsActive=true},
                        new ProductProperty{ Name="2",ParentId=shape.Id,DisplayOrder=1,IsActive=true},
                        new ProductProperty{ Name="3",ParentId=shape.Id,DisplayOrder=1,IsActive=true},
                        new ProductProperty{ Name="4",ParentId=shape.Id,DisplayOrder=1,IsActive=true},
                        new ProductProperty{ Name="5",ParentId=shape.Id,DisplayOrder=1,IsActive=true},
                        new ProductProperty{ Name="6",ParentId=shape.Id,DisplayOrder=1,IsActive=true},
                    };
                    await _db.ProductProperty.AddRangeAsync(centalcaratDTs);
                    await _db.SaveChangesAsync();

                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
