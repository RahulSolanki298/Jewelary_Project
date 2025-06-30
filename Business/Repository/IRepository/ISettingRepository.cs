using DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Repository.IRepository
{
    public interface ISettingRepository
    {
        Task<List<HomePageSetting>> GetHomePageSettingList();
        
        Task<bool> UpdateHomePageSetting(HomePageSetting homePageSetting);

        Task<HomePageSetting> GetHomePageSetting(int id);

        Task<AboutUs> GetAboutUsSetting();

        Task<bool> UpdateAboutUsSetting(AboutUs data);

        Task<List<EventSites>> GetEventSiteList();
    }
}
