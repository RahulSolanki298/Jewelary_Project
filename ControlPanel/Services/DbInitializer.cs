using Common;
using ControlPanel.Services.IServices;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            await SeedHeaders();

            await SeedProductStyles();

            await SeedProductCollections();
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
                    new Category { Name = "Bands", ProductType = "Diamond",IsActivated=true },
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
                        new ProductProperty{ Name="White",ParentId=metal.Id,SymbolName="W",DisplayOrder=1,IsActive=true,Synonyms="W,white,WG,White"},
                        new ProductProperty{ Name="Yellow",ParentId=metal.Id,SymbolName="Y",DisplayOrder=2,IsActive=true,Synonyms="Y,Yellow,yellow,YG"},
                        new ProductProperty{ Name="Rose",ParentId=metal.Id,SymbolName="R",DisplayOrder=3,IsActive=true,Synonyms="R,rose,RG,Rose"},
                        new ProductProperty{ Name="E-F",ParentId=metal.Id,SymbolName="E-F",DisplayOrder=4,IsActive=true,Synonyms="E-F"},
                    };
                    await _db.ProductProperty.AddRangeAsync(metalDTs);
                    await _db.SaveChangesAsync();


                    // Add Metals
                    var shape = await _db.ProductProperty.Where(x => x.Name == SD.Shape).FirstOrDefaultAsync();
                    var shapeDTs = new List<ProductProperty>
                    {
                        new ProductProperty{ Name="ROUND",ParentId=shape.Id,SymbolName="RND",DisplayOrder=1,IsActive=true,Synonyms="round",IconPath="/assets/img/diamond-svg/ROUND.svg"},
                        new ProductProperty{ Name="PRINCESS",ParentId=shape.Id,SymbolName="PR",DisplayOrder=2,IsActive=true,Synonyms="princess",IconPath="/assets/img/diamond-svg/PRINCESS.svg"},
                        new ProductProperty{ Name="MARQUISE",ParentId=shape.Id,SymbolName="MQ",DisplayOrder=3,IsActive=true,Synonyms="marquise",IconPath="/assets/img/diamond-svg/MARQUISE.svg"},
                        new ProductProperty{ Name="PEAR",ParentId=shape.Id,SymbolName="PEAR",DisplayOrder=4,IsActive=true,Synonyms="pear",IconPath="/assets/img/diamond-svg/PEAR.svg"},
                        new ProductProperty{ Name="HEART",ParentId=shape.Id,SymbolName="HRT",DisplayOrder=5,IsActive=true,Synonyms="heart",IconPath="/assets/img/diamond-svg/HEART.svg"},
                        new ProductProperty{ Name="OVAL",ParentId=shape.Id,SymbolName="OV",DisplayOrder=6,IsActive=true,Synonyms="oval",IconPath="/assets/img/diamond-svg/OVAL.svg"},
                        new ProductProperty{ Name="CUSHION",ParentId=shape.Id,SymbolName="CU",DisplayOrder=7,IsActive=true,Synonyms="cushion",IconPath="/assets/img/diamond-svg/CUSHION.svg"},
                        new ProductProperty{ Name="EMERALD",ParentId=shape.Id,SymbolName="EM",DisplayOrder=8,IsActive=true,Synonyms="emerald",IconPath="/assets/img/diamond-svg/Emerald.svg"},
                        new ProductProperty{ Name="RADIANT",ParentId=shape.Id,SymbolName="RAD",DisplayOrder=9,IsActive=true,Synonyms="radiant",IconPath="/assets/img/diamond-svg/RADIANT.svg"},
                        new ProductProperty{ Name="SQUARE EMERALD",ParentId=shape.Id,SymbolName="ASH",DisplayOrder=10,IsActive=true,Synonyms="sq emerald"},
                    };
                    await _db.ProductProperty.AddRangeAsync(shapeDTs);
                    await _db.SaveChangesAsync();

                    var centalcarat = await _db.ProductProperty.Where(x => x.Name == SD.CaratSize).FirstOrDefaultAsync();
                    var centalcaratDTs = new List<ProductProperty>
                    {
                        new ProductProperty{ Name="0.05",ParentId=centalcarat.Id,DisplayOrder=1,IsActive=true},
                        new ProductProperty{ Name="0.12",ParentId=centalcarat.Id,DisplayOrder=2,IsActive=true},
                        new ProductProperty{ Name="0.23",ParentId=centalcarat.Id,DisplayOrder=3,IsActive=true},
                        new ProductProperty{ Name="0.55",ParentId=centalcarat.Id,DisplayOrder=4,IsActive=true},
                        new ProductProperty{ Name="1",ParentId=centalcarat.Id,DisplayOrder=5,IsActive=true},
                        new ProductProperty{ Name="2",ParentId=centalcarat.Id,DisplayOrder=6,IsActive=true},
                        new ProductProperty{ Name="3",ParentId=centalcarat.Id,DisplayOrder=7,IsActive=true},
                        new ProductProperty{ Name="4",ParentId=centalcarat.Id,DisplayOrder=8,IsActive=true},
                        new ProductProperty{ Name="5",ParentId=centalcarat.Id,DisplayOrder=9,IsActive=true},
                        new ProductProperty{ Name="6",ParentId=centalcarat.Id,DisplayOrder=10,IsActive=true},
                    };
                    await _db.ProductProperty.AddRangeAsync(centalcaratDTs);
                    await _db.SaveChangesAsync();


                    var karatList = await _db.ProductProperty.Where(x => x.Name == SD.Karat).FirstOrDefaultAsync();
                    var karatDTs = new List<ProductProperty>
                    {
                        new ProductProperty{ Name="8K",ParentId=karatList.Id,DisplayOrder=1,IsActive=true},
                        new ProductProperty{ Name="10K",ParentId=karatList.Id,DisplayOrder=2,IsActive=true},
                        new ProductProperty{ Name="12K",ParentId=karatList.Id,DisplayOrder=3,IsActive=true},
                        new ProductProperty{ Name="14K",ParentId=karatList.Id,DisplayOrder=4,IsActive=true},
                        new ProductProperty{ Name="16K",ParentId=karatList.Id,DisplayOrder=5,IsActive=true},
                        new ProductProperty{ Name="18K",ParentId=karatList.Id,DisplayOrder=6,IsActive=true},
                        new ProductProperty{ Name="20K",ParentId=karatList.Id,DisplayOrder=7,IsActive=true},
                        new ProductProperty{ Name="22K",ParentId=karatList.Id,DisplayOrder=8,IsActive=true},
                        new ProductProperty{ Name="24K",ParentId=karatList.Id,DisplayOrder=9,IsActive=true},
                    };
                    await _db.ProductProperty.AddRangeAsync(karatDTs);
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
                        new DiamondProperty{ Name="D",ParentId=color.Id,ColorType="Colorless",Description="Highest quality, rare",DispOrder=1,IsActivated=true},
                        new DiamondProperty{ Name="E",ParentId=color.Id,ColorType="Colorless",Description="Slight trace of color only detectable by gemologists\r\n",DispOrder=2,IsActivated=true},
                        new DiamondProperty{ Name="F",ParentId=color.Id,ColorType="Colorless",Description="Still very high quality\r\n",DispOrder=3,IsActivated=true},
                        new DiamondProperty{ Name="G",ParentId=color.Id,ColorType="Near Colorless",Description="Slight warmth, very slight color\r\n",DispOrder=4,IsActivated=true},
                        new DiamondProperty{ Name="H",ParentId=color.Id,ColorType="Near Colorless",Description="Noticeable color only when compared\r\n",DispOrder=5,IsActivated=true},
                        new DiamondProperty{ Name="I",ParentId=color.Id,ColorType="Near Colorless",Description="Slightly tinted\r\n",DispOrder=6,IsActivated=true},
                        new DiamondProperty{ Name="J",ParentId=color.Id,ColorType="Near Colorless",Description="Some warmth visible\r\n",DispOrder=7,IsActivated=true},
                        new DiamondProperty{ Name="K",ParentId=color.Id,ColorType="Faint",Description="Faint yellow or brown tint\r\n",DispOrder=8,IsActivated=true},
                        new DiamondProperty{ Name="L",ParentId=color.Id,ColorType="Faint",Description = "More visible tint\r\n", DispOrder=9,IsActivated=true},
                        new DiamondProperty{ Name="M",ParentId=color.Id,ColorType="Faint",Description="Starting to be noticeable to the eye\r\n",DispOrder=10,IsActivated=true},
                        new DiamondProperty{ Name="N",ParentId=color.Id,ColorType="Very Light",Description = "Noticeable tint", DispOrder=11,IsActivated=true},
                        new DiamondProperty{ Name="O",ParentId=color.Id,ColorType="Very Light",Description = "Noticeable tint",DispOrder=12,IsActivated=true},
                        new DiamondProperty{ Name="P",ParentId=color.Id,ColorType="Very Light",Description = "Noticeable tint",DispOrder=13,IsActivated=true},
                        new DiamondProperty{ Name="Q",ParentId=color.Id,ColorType="Very Light",Description = "Noticeable tint",DispOrder=14,IsActivated=true},
                        new DiamondProperty{ Name="R",ParentId=color.Id,ColorType="Very Light",Description = "Noticeable tint",DispOrder=15,IsActivated=true},
                        new DiamondProperty{ Name="S",ParentId=color.Id,ColorType="Light",Description="Clearly visible color",DispOrder=16,IsActivated=true},
                        new DiamondProperty{ Name="T",ParentId=color.Id,ColorType="Light",Description="Clearly visible color",DispOrder=17,IsActivated=true},
                        new DiamondProperty{ Name="U",ParentId=color.Id,ColorType="Light",Description="Clearly visible color",DispOrder=18,IsActivated=true},
                        new DiamondProperty{ Name="V",ParentId=color.Id,ColorType="Light",Description="Clearly visible color",DispOrder=19,IsActivated=true},
                        new DiamondProperty{ Name="W",ParentId=color.Id,ColorType="Light",Description="Clearly visible color",DispOrder=20,IsActivated=true},
                        new DiamondProperty{ Name="X",ParentId=color.Id,ColorType="Light",Description="Clearly visible color",DispOrder=21,IsActivated=true},
                        new DiamondProperty{ Name="Y",ParentId=color.Id,ColorType="Light",Description="Clearly visible color",DispOrder=22,IsActivated=true},
                        new DiamondProperty{ Name="Z",ParentId=color.Id,ColorType="Light",Description="Clearly visible color",DispOrder=23,IsActivated=true},
                    };
                    await _db.DiamondProperties.AddRangeAsync(colorDTs);
                    await _db.SaveChangesAsync();

                    var shape = await _db.DiamondProperties.Where(x => x.Name == SD.Shape).FirstOrDefaultAsync();
                    var ShapesDTs = new List<DiamondProperty>
                    {
                        new DiamondProperty{ Name="Round",SymbolName="RND",ParentId=shape.Id,DispOrder=1,IsActivated=true,IconPath="/assets/img/diamond-svg/ROUND.svg"},
                        new DiamondProperty{ Name="Princess",SymbolName="PR",ParentId=shape.Id,DispOrder=2,IsActivated=true,IconPath="/assets/img/diamond-svg/PRINCESS.svg"},
                        new DiamondProperty{ Name="Oval",SymbolName="OV",ParentId=shape.Id,DispOrder=3,IsActivated=true,IconPath="/assets/img/diamond-svg/OVAL.svg"},
                        new DiamondProperty{ Name="Cushion",SymbolName="CU",ParentId=shape.Id,DispOrder=4,IsActivated=true,IconPath="/assets/img/diamond-svg/CUSHION.svg"},
                        new DiamondProperty{ Name="Radiant",SymbolName="RND",ParentId=shape.Id,DispOrder=5,IsActivated=true,IconPath="/assets/img/diamond-svg/ROUND.svg"},
                        //new DiamondProperty{ Name="Elongated Cushion",SymbolName="RND",ParentId=shape.Id,DispOrder=6,IsActivated=true},
                        new DiamondProperty{ Name="Pear",SymbolName = "PE", ParentId=shape.Id,DispOrder=6,IsActivated=true,IconPath="/assets/img/diamond-svg/PEAR.svg"},
                        new DiamondProperty{ Name="EmeraId",SymbolName="EM",ParentId=shape.Id,DispOrder=7,IsActivated=true,IconPath="/assets/img/diamond-svg/Emerald.svg"},
                        new DiamondProperty{ Name="Asscher",SymbolName="ASH",ParentId=shape.Id,DispOrder=8,IsActivated=true},
                        new DiamondProperty{ Name="Marquise",SymbolName = "MQ", ParentId=shape.Id,DispOrder=9,IsActivated=true,IconPath="/assets/img/diamond-svg/MARQUISE.svg"},
                        new DiamondProperty{ Name="Heart",SymbolName="HRT",ParentId=shape.Id,DispOrder=10,IsActivated=true,IconPath="/assets/img/diamond-svg/HEART.svg"},
                    };
                    await _db.DiamondProperties.AddRangeAsync(ShapesDTs);
                    await _db.SaveChangesAsync();

                    var lab = await _db.DiamondProperties.Where(x => x.Name == SD.Lab).FirstOrDefaultAsync();
                    var LabDTs = new List<DiamondProperty>
                    {
                        new DiamondProperty{ Name="IGI",Description="International Gemological Institute",SymbolName="IGI",ParentId=lab.Id,DispOrder=1,IsActivated=true,IconPath="/assets/img/IGI.png"},
                        new DiamondProperty{ Name="GCAL",Description="Gem Certification & Assurance Lab",ParentId=lab.Id,DispOrder=2,IsActivated=false},
                        new DiamondProperty{ Name="GIA",Description="Gemological Institute of America",SymbolName="GIA",ParentId=lab.Id,DispOrder=3,IsActivated=true,IconPath="/assets/img/diamond-svg/IGI.png"},
                        new DiamondProperty{ Name="AGS",Description="American Gem Society",ParentId=lab.Id,DispOrder=4,IsActivated=false},
                        new DiamondProperty{ Name="HRD",Description="Hoge Raad voor Diamant (Belgium)\t",ParentId=lab.Id,DispOrder=5,IsActivated=false},
                        new DiamondProperty{ Name="EGL",Description = "European Gemological Laboratory\t", ParentId=lab.Id,DispOrder=6,IsActivated=false},
                    };
                    await _db.DiamondProperties.AddRangeAsync(LabDTs);
                    await _db.SaveChangesAsync();

                    var clarity = await _db.DiamondProperties.Where(x => x.Name == SD.Clarity).FirstOrDefaultAsync();
                    var ClarityDTs = new List<DiamondProperty>
                    {
                        new DiamondProperty { Name = "FL", Description = "Flawless (FL): No inclusions or blemishes visible under 10x magnification.", ParentId = clarity.Id, DispOrder = 1, IsActivated = true, IconPath = "/assets/img/clarities/FL.png" },
                        new DiamondProperty { Name = "IF", Description = "Internally Flawless (IF): No internal inclusions, but may have surface blemishes.", ParentId = clarity.Id, DispOrder = 2, IsActivated = true, IconPath = "/assets/img/clarities/IF.png" },
                        new DiamondProperty { Name = "VVS1", Description = "Very, Very Slightly Included (VVS1): Inclusions are extremely difficult to see under 10x magnification.", ParentId = clarity.Id, DispOrder = 3, IsActivated = true, IconPath = "/assets/img/clarities/VVS1.png" },
                        new DiamondProperty { Name = "VVS2", Description = "Very, Very Slightly Included (VVS2): Inclusions are very difficult to see under 10x magnification.", ParentId = clarity.Id, DispOrder = 4, IsActivated = true, IconPath = "/assets/img/clarities/VVS2.png" },
                        new DiamondProperty { Name = "VS1", Description = "Very Slightly Included (VS1): Inclusions are minor and difficult to detect with 10x magnification.", ParentId = clarity.Id, DispOrder = 5, IsActivated = true, IconPath = "/assets/img/clarities/VS1.png" },
                        new DiamondProperty { Name = "VS2", Description = "Very Slightly Included (VS2): Inclusions are more noticeable under magnification.", ParentId = clarity.Id, DispOrder = 6, IsActivated = true, IconPath = "/assets/img/clarities/VS2.png" },
                        new DiamondProperty { Name = "SI1", Description = "Slightly Included (SI1): Inclusions are noticeable under 10x magnification and may be visible to the naked eye.", ParentId = clarity.Id, DispOrder = 7, IsActivated = true, IconPath = "/assets/img/clarities/SI1.png" },
                        new DiamondProperty { Name = "SI2", Description = "Slightly Included (SI2): Inclusions are visible to the naked eye.", ParentId = clarity.Id, DispOrder = 8, IsActivated = true, IconPath = "/assets/img/clarities/SI2.png" },
                        new DiamondProperty { Name = "I1", Description = "Included (I1): Inclusions are obvious under 10x magnification and may affect the diamond's transparency or brilliance.", ParentId = clarity.Id, DispOrder = 9, IsActivated = true, IconPath = "/assets/img/clarities/I1.png" },
                        new DiamondProperty { Name = "I2", Description = "Included (I2): Inclusions are significant and visible to the naked eye, potentially affecting the diamond’s appearance.", ParentId = clarity.Id, DispOrder = 10, IsActivated = true, IconPath = "/assets/img/clarities/I2.png" },
                        new DiamondProperty { Name = "I3", Description = "Included (I3): Inclusions are so severe that they affect the diamond’s overall appearance, brilliance, and durability.", ParentId = clarity.Id, DispOrder = 11, IsActivated = true, IconPath = "/assets/img/clarities/I3.png" }
                    };
                    await _db.DiamondProperties.AddRangeAsync(ClarityDTs);
                    await _db.SaveChangesAsync();

                }
                catch (Exception)
                {

                    throw;
                }
            }

            if (!await _db.ProductProperty.AnyAsync())
            {
                try
                {
                    // Add Metals
                    var pshape = await _db.ProductProperty.Where(x => x.Name == SD.Shape).FirstOrDefaultAsync();
                    var pshapeDTs = new List<ProductProperty>
                    {
                        new ProductProperty{ Name="ROUND",ParentId=pshape.Id,SymbolName="round",DisplayOrder=1,IsActive=true,Synonyms="round",IconPath="/assets/img/diamond-svg/ROUND.svg"},
                        new ProductProperty{ Name="PRINCESS",ParentId=pshape.Id,SymbolName="princess",DisplayOrder=2,IsActive=true,Synonyms="princess",IconPath="/assets/img/diamond-svg/PRINCESS.svg"},
                        new ProductProperty{ Name="MARQUISE",ParentId=pshape.Id,SymbolName="marquise",DisplayOrder=3,IsActive=true,Synonyms="marquise",IconPath="/assets/img/diamond-svg/MARQUISE.svg"},
                        new ProductProperty{ Name="PEAR",ParentId=pshape.Id,SymbolName="pear",DisplayOrder=4,IsActive=true,Synonyms="pear",IconPath="/assets/img/diamond-svg/PEAR.svg"},
                        new ProductProperty{ Name="HEART",ParentId=pshape.Id,SymbolName="heart",DisplayOrder=5,IsActive=true,Synonyms="heart",IconPath="/assets/img/diamond-svg/HEART.svg"},
                        new ProductProperty{ Name="OVAL",ParentId=pshape.Id,SymbolName="oval",DisplayOrder=6,IsActive=true,Synonyms="oval",IconPath="/assets/img/diamond-svg/OVAL.svg"},
                        new ProductProperty{ Name="CUSHION",ParentId=pshape.Id,SymbolName="cushion",DisplayOrder=7,IsActive=true,Synonyms="cushion",IconPath="/assets/img/diamond-svg/CUSHION.svg"},
                        new ProductProperty{ Name="EMERALD",ParentId=pshape.Id,SymbolName="emerald",DisplayOrder=8,IsActive=true,Synonyms="emerald",IconPath="/assets/img/diamond-svg/Emerald.svg"},
                        new ProductProperty{ Name="RADIANT",ParentId=pshape.Id,SymbolName="radiant",DisplayOrder=9,IsActive=true,Synonyms="radiant",IconPath="/assets/img/diamond-svg/RADIANT.svg"},
                        new ProductProperty{ Name="SQ EMERALD",ParentId=pshape.Id,SymbolName="sq emerald",DisplayOrder=10,IsActive=true,Synonyms="sq emerald"},
                    };
                    await _db.ProductProperty.AddRangeAsync(pshapeDTs);
                    await _db.SaveChangesAsync();

                    var centalcarat = await _db.ProductProperty.Where(x => x.Name == SD.CaratSize).FirstOrDefaultAsync();
                    var centalcaratDTs = new List<ProductProperty>
                    {
                        new ProductProperty{ Name="0.05",ParentId=centalcarat.Id,DisplayOrder=1,IsActive=true},
                        new ProductProperty{ Name="0.12",ParentId=centalcarat.Id,DisplayOrder=2,IsActive=true},
                        new ProductProperty{ Name="0.23",ParentId=centalcarat.Id,DisplayOrder=3,IsActive=true},
                        new ProductProperty{ Name="0.55",ParentId=centalcarat.Id,DisplayOrder=4,IsActive=true},
                        new ProductProperty{ Name="1",ParentId=centalcarat.Id,DisplayOrder=5,IsActive=true},
                        new ProductProperty{ Name="2",ParentId=centalcarat.Id,DisplayOrder=6,IsActive=true},
                        new ProductProperty{ Name="3",ParentId=centalcarat.Id,DisplayOrder=7,IsActive=true},
                        new ProductProperty{ Name="4",ParentId=centalcarat.Id,DisplayOrder=8,IsActive=true},
                        new ProductProperty{ Name="5",ParentId=centalcarat.Id,DisplayOrder=9,IsActive=true},
                        new ProductProperty{ Name="6",ParentId=centalcarat.Id,DisplayOrder=10,IsActive=true},
                    };
                    await _db.ProductProperty.AddRangeAsync(centalcaratDTs);
                    await _db.SaveChangesAsync();


                    //
                    var lab = await _db.ProductProperty.Where(x => x.Name == SD.Lab).FirstOrDefaultAsync();
                    var LabDTs = new List<ProductProperty>
                    {
                        new ProductProperty{ Name="IGI",Description="International Gemological Institute",SymbolName="IGI",ParentId=lab.Id,DisplayOrder=1,IsActive=true,IconPath="/assets/img/IGI.png"},
                        new ProductProperty{ Name="GCAL",Description="Gem Certification & Assurance Lab",ParentId=lab.Id,DisplayOrder=2,IsActive=false},
                        new ProductProperty{ Name="GIA",Description="Gemological Institute of America",SymbolName="GIA",ParentId=lab.Id,DisplayOrder=3,IsActive=true,IconPath="/assets/img/diamond-svg/IGI.png"},
                        new ProductProperty{ Name="AGS",Description="American Gem Society",ParentId=lab.Id,DisplayOrder=4,IsActive=false},
                        new ProductProperty{ Name="HRD",Description="Hoge Raad voor Diamant (Belgium)",ParentId=lab.Id,DisplayOrder=5,IsActive=false},
                        new ProductProperty{ Name="EGL",Description = "European Gemological Laboratory", ParentId=lab.Id,DisplayOrder=6,IsActive=false},
                    };
                    await _db.ProductProperty.AddRangeAsync(LabDTs);
                    await _db.SaveChangesAsync();

                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        private async Task SeedProductStyles()
        {
            if (!await _db.ProductStyles.AnyAsync())
            {
                var ring = await _db.Category.Where(x => x.Name == SD.Rings).FirstOrDefaultAsync();
                var bands = await _db.Category.Where(x => x.Name == SD.Bands).FirstOrDefaultAsync();
                var earring = await _db.Category.Where(x => x.Name == SD.Earrings).FirstOrDefaultAsync();
                var nacklece = await _db.Category.Where(x => x.Name == SD.Nackleces).FirstOrDefaultAsync();
                var pendant = await _db.Category.Where(x => x.Name == SD.Pendants).FirstOrDefaultAsync();
                var bracelet = await _db.Category.Where(x => x.Name == SD.Bracelets).FirstOrDefaultAsync();
                var bangle = await _db.Category.Where(x => x.Name == SD.Bangle).FirstOrDefaultAsync();

                var stylesDT = new List<ProductStyles>
                {
                    new ProductStyles { StyleName = "Solitaire",StyleImage="/assets/icons/solitaire.png" ,CategoryId = ring.Id >0?ring.Id : null ,IsActivated=true,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now },
                    new ProductStyles { StyleName = "Side Stone",StyleImage="/assets/icons/side-stone.png" , CategoryId = ring.Id ,IsActivated=true,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now },
                    new ProductStyles { StyleName = "Halo",StyleImage="/assets/icons/halo.png" , CategoryId = ring.Id ,IsActivated=true,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now },
                    new ProductStyles { StyleName = "Three Stone",StyleImage="/assets/icons/three-stone.png" , CategoryId = ring.Id ,IsActivated=true,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now },
                    new ProductStyles { StyleName = "Hidden Halo",StyleImage="/assets/icons/hidden-halo.png" , CategoryId = ring.Id ,IsActivated=true ,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now},
                    new ProductStyles { StyleName = "Vintage",StyleImage="/assets/icons/vintage.png" , CategoryId = ring.Id ,IsActivated=true ,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now},
                    new ProductStyles { StyleName = "Bridal Sets",StyleImage="/assets/icons/bridal-sets.png" , CategoryId = ring.Id ,IsActivated=true,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now },
                    new ProductStyles { StyleName = "Fashion-bracelet",StyleImage="/assets/icons/fashion-bracelet.png" , CategoryId = bracelet.Id ,IsActivated=true,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now },
                    new ProductStyles { StyleName = "Fashion-Earring",StyleImage="/assets/icons/fashion-earring.png" , CategoryId = earring.Id ,IsActivated=true,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now },
                    new ProductStyles { StyleName = "Fashion-Ring",StyleImage="/assets/icons/fashion-ring.png" , CategoryId = ring.Id ,IsActivated=true,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now },
                    new ProductStyles { StyleName = "Hidden-halo",StyleImage="/assets/icons/hidden-halo.png" , CategoryId = ring.Id ,IsActivated=true,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now },
                    new ProductStyles { StyleName = "Eternity-Bands-Small",StyleImage="/assets/icons/eternity-bands-small.png" , CategoryId = bands.Id ,IsActivated=true,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now },
                    new ProductStyles { StyleName = "Eternity-ring",StyleImage="/assets/icons/hidden-ring.png" , CategoryId = ring.Id ,IsActivated=true,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now },
                    
                    //new ProductStyles { StyleName = "Wedding and Anniversary", CategoryId = bands.Id ,IsActivated=true ,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now},
                    //new ProductStyles { StyleName = "Eternity", CategoryId = bands.Id ,IsActivated=true ,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now},
                    //new ProductStyles { StyleName = "3/4 Eternity", CategoryId = bands.Id ,IsActivated=true ,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now},
                    //new ProductStyles { StyleName = "Enhancers", CategoryId = bands.Id ,IsActivated=true ,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now},
                    //new ProductStyles { StyleName = "Curve Bands", CategoryId = bands.Id ,IsActivated=true ,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now},
                    //new ProductStyles { StyleName = "Open Bands", CategoryId = bands.Id ,IsActivated=true ,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now},
                    //new ProductStyles { StyleName = "Design Your Stack", CategoryId = bands.Id ,IsActivated=true,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now },
                    //new ProductStyles { StyleName = "Customer Favorites", CategoryId = bands.Id ,IsActivated=true,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now },
                };
                await _db.ProductStyles.AddRangeAsync(stylesDT);
                await _db.SaveChangesAsync();
            }
        }

        private async Task SeedProductCollections()
        {
            if (!await _db.ProductCollections.AnyAsync())
            {
                var collection = new List<ProductCollections>
                {
                    new ProductCollections { CollectionName = "Curve", IsActivated=true,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now },
                    new ProductCollections { CollectionName = "Muse", IsActivated=true,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now },
                    new ProductCollections { CollectionName = "Nostalgia", IsActivated=true,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now },
                    new ProductCollections { CollectionName = "Mosaic", IsActivated=true,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now },
                    new ProductCollections { CollectionName = "Boundless", IsActivated=true ,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now},
                    new ProductCollections { CollectionName = "Toi Et Moi", IsActivated=true ,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now},
                    new ProductCollections { CollectionName = "Naas", IsActivated=true,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now },
                    new ProductCollections { CollectionName = "High Jewelry", IsActivated=true,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now },
                    new ProductCollections { CollectionName = "Aether Diamonds", IsActivated=true,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now },
                };
                await _db.ProductCollections.AddRangeAsync(collection);
                await _db.SaveChangesAsync();
            }
        }

        private async Task SeedHeaders()
        {
            if (!await _db.HomePageSetting.AnyAsync())
            {
                var collection = new HomePageSetting
                {
                    CompanyLogo = "/assets/img/Jewelfacets_Logos_Blue.png",
                    Device = SD.WebDevice,
                    isSetVideo = true,
                    SetVideoPath = "/assets/video/jewel-facet-banner.mp4"
                };
                await _db.HomePageSetting.AddRangeAsync(collection);
                await _db.SaveChangesAsync();
            }
        }
    }
}
