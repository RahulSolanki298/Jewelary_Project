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
            if (id.HasValue && id.Value > 0)
            {
                home = await _settingRepository.GetHomePageSetting(id.Value);
            }
            return PartialView("_HomePageSetting", home);
        }

        [HttpPost]
        public async Task<IActionResult> SaveHomePageSetting(HomePageSetting model)
        {
            var uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            if (!Directory.Exists(uploadRoot))
                Directory.CreateDirectory(uploadRoot);

            if (model != null)
            {
                var existingSettings = await _settingRepository.GetHomePageSetting(model.Id);

                // Update logo if a new one is uploaded
                if (model.CompanyLogoFile != null)
                {
                    model.CompanyLogo = await SaveFileAsync(model.CompanyLogoFile, uploadRoot);
                }
                else
                {
                    model.CompanyLogo = existingSettings.CompanyLogo;
                }

                // Update video if a new one is uploaded
                if (model.isSetVideo && model.VideoFile != null && model.VideoFile.Length > 0)
                {
                    model.SetVideoPath = await SaveFileAsync(model.VideoFile, uploadRoot);
                }
                else
                {
                    model.SetVideoPath = existingSettings.SetVideoPath;
                }

                // Update sliders only if new ones are uploaded
                if (model.isSetCompanySlider)
                {
                    model.SetSlider1Path = model.Image1Path != null
                        ? await SaveFileAsync(model.Image1Path, uploadRoot)
                        : existingSettings.SetSlider1Path;

                    model.SetSlider2Path = model.Image2Path != null
                        ? await SaveFileAsync(model.Image2Path, uploadRoot)
                        : existingSettings.SetSlider2Path;

                    model.SetSlider3Path = model.Image3Path != null
                        ? await SaveFileAsync(model.Image3Path, uploadRoot)
                        : existingSettings.SetSlider3Path;
                }
                else
                {
                    if (existingSettings != null)
                    {
                        model.SetSlider1Path = existingSettings.SetSlider1Path != null ? existingSettings.SetSlider1Path : "-";
                        model.SetSlider2Path = existingSettings.SetSlider2Path != null ? existingSettings.SetSlider2Path : "-";
                        model.SetSlider3Path = existingSettings.SetSlider3Path != null ? existingSettings.SetSlider3Path : "-";
                    }
                }

                // Save updated model to DB
                await _settingRepository.UpdateHomePageSetting(model);
            }

            TempData["Message"] = "Settings saved successfully!";
            return RedirectToAction("Index");
        }


        private async Task<string> SaveFileAsync(IFormFile file, string rootPath)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(rootPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative URL path
            return $"/uploads/{fileName}";
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
            if (about != true)
            {
                return Json("Data updated failed.");
            }
            return Json("Data updated successfully");
        }

        [HttpGet]
        public IActionResult CreateEventBaseDiscount()
        {
            var eventBase = new EventSites();
            return PartialView("_EventBaseDiscount", eventBase);
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
