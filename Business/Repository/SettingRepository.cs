using Business.Repository.IRepository;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
            int[] categoryIds = { },styleIds = { },collectionIds = { };
            var response = await _context.HomePageSetting.FirstOrDefaultAsync(x => x.Id == id);

            response.CategoryList = await _context.Category.Where(x => x.IsActivated == true).ToListAsync();
            response.StyleList = await _context.ProductStyles.Where(x => x.IsActivated == true).ToListAsync();
            response.CollectionList = await _context.ProductCollections.Where(x => x.IsActivated == true).ToListAsync();
            //if (!string.IsNullOrEmpty(response.CategoryIds)) categoryIds = response.CategoryIds.Split(',').Select(int.Parse).ToArray();
            //if (categoryIds.Length > 0) response.CategoryList = await _context.Category.Where(x=>x.IsActivated==true && categoryIds.Contains(x.Id)).ToListAsync();

            //if (!string.IsNullOrEmpty(response.StylesIds)) styleIds = response.StylesIds.Split(',').Select(int.Parse).ToArray();
            //if (styleIds.Length > 0) response.StyleList = await _context.ProductStyles.Where(x => x.IsActivated == true && styleIds.Contains(x.Id)).ToListAsync();

            //if (!string.IsNullOrEmpty(response.CollectionIds)) collectionIds = response.CollectionIds.Split(',').Select(int.Parse).ToArray();
            //if (collectionIds.Length > 0) response.CollectionList = await _context.ProductCollections.Where(x => x.IsActivated == true && collectionIds.Contains(x.Id)).ToListAsync();

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
                    response.SetVideoPath = !string.IsNullOrEmpty(homePageSetting.SetVideoPath) ? homePageSetting.SetVideoPath : response.SetVideoPath;
                    response.VdoTitle = !string.IsNullOrEmpty(homePageSetting.VdoTitle) ? homePageSetting.VdoTitle : response.VdoTitle;
                    response.VdoMetaUrl =!string.IsNullOrEmpty(homePageSetting.VdoMetaUrl) ? homePageSetting.VdoMetaUrl : response.VdoMetaUrl;

                    response.isSetCompanySlider = homePageSetting.isSetCompanySlider ? homePageSetting.isSetCompanySlider : response.isSetCompanySlider;
                    response.SetSlider1Path = !string.IsNullOrEmpty(homePageSetting.SetSlider1Path) ? homePageSetting.SetSlider1Path : response.SetSlider1Path;
                    response.Slider1MetaUrl = !string.IsNullOrEmpty(homePageSetting.Slider1MetaUrl) ? homePageSetting.Slider1MetaUrl : response.Slider1MetaUrl;
                    response.Slider1Title = !string.IsNullOrEmpty(homePageSetting.Slider1Title) ? homePageSetting.Slider1Title : response.Slider1Title;

                    response.SetSlider2Path = !string.IsNullOrEmpty(homePageSetting.SetSlider2Path) ? homePageSetting.SetSlider2Path : response.SetSlider2Path;
                    response.Slider2MetaUrl = !string.IsNullOrEmpty(homePageSetting.Slider2MetaUrl) ? homePageSetting.Slider2MetaUrl : response.Slider2MetaUrl;
                    response.Slider2Title = !string.IsNullOrEmpty(homePageSetting.Slider2Title) ? homePageSetting.Slider2Title : response.Slider2Title;

                    response.SetSlider3Path = !string.IsNullOrEmpty(homePageSetting.SetSlider3Path) ? homePageSetting.SetSlider3Path : response.SetSlider3Path;
                    response.Slider3MetaUrl = !string.IsNullOrEmpty(homePageSetting.Slider3MetaUrl) ? homePageSetting.Slider3MetaUrl : response.Slider3MetaUrl;
                    response.Slider3Title = !string.IsNullOrEmpty(homePageSetting.Slider3Title) ? homePageSetting.Slider3Title : response.Slider3Title;
                    
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

        public async Task<List<EventSites>> GetEventSiteList()
        {
            var response = await _context.EventSites.ToListAsync();
            return response;
        }
    }
}
