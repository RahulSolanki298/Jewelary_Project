using Business.Repository.IRepository;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task<HomePageSetting> GetHomePageSetting()
        {
            var response = await _context.HomePageSetting.FirstOrDefaultAsync();
            return response;
        }

        public async Task<bool> UpdateHomePageSetting(HomePageSetting homePageSetting)
        {
            try
            {
                var response = await _context.HomePageSetting.FirstOrDefaultAsync();
                if (response != null)
                {
                    response.isSetVideo = homePageSetting.isSetVideo;
                    response.VideoFile = homePageSetting.VideoFile;

                    response.isSetCompanySlider = homePageSetting.isSetCompanySlider;
                    response.Image1Path = homePageSetting.Image1Path;
                    response.Image2Path = homePageSetting.Image2Path;
                    response.Image3Path = homePageSetting.Image3Path;
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
    }
}
