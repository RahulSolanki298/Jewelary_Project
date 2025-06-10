using Business.Repository.IRepository;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Repository
{
    public class SettingRepository : ISettingRepository
    {
        private readonly ApplicationDBContext _context;
        public SettingRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<HomePageSetting> GetHomePageSetting(int id)
        {
            var response = await _context.HomePageSetting.FirstOrDefaultAsync(x => x.Id == id);
            return response;
        }

        public async Task<bool> UpdateHomePageSetting(HomePageSetting homePageSetting)
        {
            try
            {
                var response = await _context.HomePageSetting.FirstOrDefaultAsync();
                if (response != null)
                {
                    response.CompanyLogo = homePageSetting.CompanyLogo;
                    response.Device = homePageSetting.Device;
                    response.isSetVideo = homePageSetting.isSetVideo;
                    response.VideoFile = homePageSetting.VideoFile;

                    response.isSetCompanySlider = homePageSetting.isSetCompanySlider;
                    response.SetSlider1Path = homePageSetting.SetSlider1Path;
                    response.SetSlider2Path = homePageSetting.SetSlider2Path;
                    response.SetSlider3Path = homePageSetting.SetSlider3Path;
                    _context.HomePageSetting.Update(response);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    await _context.HomePageSetting.AddAsync(homePageSetting);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }


        }

        public async Task<List<HomePageSetting>> GetHomePageSettingList()
        {
            var response = await _context.HomePageSetting.ToListAsync();
            return response;
        }

        public async Task<AboutUs> GetAboutUsSetting()
        {
            var response = await _context.AboutUs.FirstOrDefaultAsync();
            return response;
        }


        public async Task<bool> UpdateAboutUsSetting(AboutUs data)
        {
            try
            {
                var response = await _context.AboutUs.FirstOrDefaultAsync();
                if (response != null)
                {
                    response.Description = data.Description;
                    _context.AboutUs.Update(response);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    await _context.AboutUs.AddAsync(new AboutUs
                    {
                        Description = data.Description
                    });
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
                return false;
            }
        }
    }
}
