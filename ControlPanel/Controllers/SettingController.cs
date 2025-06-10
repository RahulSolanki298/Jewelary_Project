using Business.Repository.IRepository;
using DataAccess.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ControlPanel.Controllers
{
    public class SettingController : Controller
    {
        private readonly ISettingRepository _settingRepository;
        public SettingController(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> HomePageSettingList()
        {
            var homeSettingList = await _settingRepository.GetHomePageSettingList();
            return PartialView("_HomePageSettingList", homeSettingList);
        }

        [HttpGet]
        public async Task<IActionResult> CreateHome(int? id)
        {
            var home = new HomePageSetting();
            if (home != null)
            {
                home = await _settingRepository.GetHomePageSetting(id.Value);
            }
            return PartialView("_HomePageSetting", home);
        }

        [HttpPost]
        public async Task<IActionResult> SaveHomePageSetting(HomePageSetting model)
        {
            if (model.CompanyLogoFile != null)
            {
                var path1 = Path.Combine("wwwroot/uploads", model.CompanyLogoFile.FileName);
                using var fs1 = new FileStream(path1, FileMode.Create);
                await model.CompanyLogoFile.CopyToAsync(fs1);
                model.CompanyLogo = "/uploads/" + model.CompanyLogoFile.FileName;
            }

            if (model.isSetVideo && model.VideoFile != null)
            {
                // Save video
                var videoPath = Path.Combine("wwwroot/uploads", model.VideoFile.FileName);
                using (var stream = new FileStream(videoPath, FileMode.Create))
                {
                    await model.VideoFile.CopyToAsync(stream);
                }
                model.SetVideoPath = "/uploads/" + model.VideoFile.FileName;
            }

            if (model.isSetCompanySlider)
            {
                if (model.Image1Path != null)
                {
                    var path1 = Path.Combine("wwwroot/uploads", model.Image1Path.FileName);
                    using var fs1 = new FileStream(path1, FileMode.Create);
                    await model.Image1Path.CopyToAsync(fs1);
                    model.SetSlider1Path = "/uploads/" + model.Image1Path.FileName;
                }

                if (model.Image2Path != null)
                {
                    var path2 = Path.Combine("wwwroot/uploads", model.Image2Path.FileName);
                    using var fs2 = new FileStream(path2, FileMode.Create);
                    await model.Image2Path.CopyToAsync(fs2);
                    model.SetSlider2Path = "/uploads/" + model.Image2Path.FileName;
                }

                if (model.Image3Path != null)
                {
                    var path3 = Path.Combine("wwwroot/uploads", model.Image3Path.FileName);
                    using var fs3 = new FileStream(path3, FileMode.Create);
                    await model.Image3Path.CopyToAsync(fs3);
                    model.SetSlider3Path = "/uploads/" + model.Image3Path.FileName;
                }
            }

            await _settingRepository.UpdateHomePageSetting(model);


            TempData["Message"] = "Settings saved successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> AboutUs()
        {
            var about = await _settingRepository.GetAboutUsSetting();
            return PartialView("_AboutUs", about);
        }

        [HttpPost]
        public async Task<IActionResult> AboutUs(AboutUs aboutUs)
        {
            var about = await _settingRepository.UpdateAboutUsSetting(aboutUs);
            if (about!=true)
            {
                return Json("Data updated failed.");
            }
            return Json("Data updated successfully");
        }

        [HttpGet]
        public IActionResult CreateEventBaseDiscount()
        {
            var eventBase = new EventSites();
            return PartialView("_EventBaseDiscount",eventBase);
        }


        [HttpPost]
        public async Task<IActionResult> CreateEventBaseDiscount(EventSites model, IFormFile imageFile)
        {
            try
            {
                if (imageFile != null)
                {
                    var uploadsDir = Path.Combine("wwwroot/uploads/events");
                    Directory.CreateDirectory(uploadsDir);
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(uploadsDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    model.ProfileImage = "/uploads/events/" + fileName;
                }

                // Save to database here...
                

                return Json(new { success = true, message = "Event created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }



        public IActionResult InquiryAndVirtualMeeting()
        {

            return View();
        }
    }
}
