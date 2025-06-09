using DataAccess.Entities;
using System.Threading.Tasks;

namespace Business.Repository.IRepository
{
    public interface ISettingRepository
    {
        Task<HomePageSetting> GetHomePageSetting();
        Task<bool> UpdateHomePageSetting(HomePageSetting homePageSetting);

    }
}
