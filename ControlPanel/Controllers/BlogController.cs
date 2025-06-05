using Business.Repository.IRepository;
using DataAccess.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ControlPanel.Controllers
{
    public class BlogController : Controller
    {
        public readonly IBlogRepository _blogRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public readonly ILogEntryRepository _logEntryRepository;
        public BlogController(IBlogRepository blogRepository, ILogEntryRepository logEntryRepository, UserManager<ApplicationUser> userManager)
        {
            _blogRepository = blogRepository;
            _logEntryRepository = logEntryRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> getBlogList()
        {
            bool status = false;
            string strResult = "";
            string strMessage = "Data Not Found";

            var data = await _blogRepository.GetBlogList();
            if (data != null)
            {
                status = true;
                strMessage = "";
                strResult = JsonConvert.SerializeObject(data);
            }
            return Json(new
            {
                Data = new
                {
                    status = status,
                    result = strResult,
                    message = strMessage
                }
            });
        }


        [HttpGet]
        public async Task<IActionResult> GetBlogById(int? id = 0)
        {
            var blog = new Blogs();
            if (id > 0) blog = await _blogRepository.GetBlogById(id.Value);
            return View(blog);
        }

        [HttpPost]
        public async Task<IActionResult> GetBlogById(Blogs blog)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var user = await _userManager.FindByIdAsync(userId);

            if (!ModelState.IsValid)
            {
                TempData["Status"] = "Fail";
                TempData["Message"] = "Invalid blog data submitted.";
                return View(blog);
            }

            try
            {
                if (blog.ImageFile != null && blog.ImageFile.Length > 0)
                {
                    // Set unique file name
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(blog.ImageFile.FileName)}";
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    if (!Directory.Exists(uploads))
                        Directory.CreateDirectory(uploads);

                    var filePath = Path.Combine(uploads, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await blog.ImageFile.CopyToAsync(stream);
                    }

                    blog.BlogImage = $"/uploads/{fileName}"; // Save path for display
                }
                blog.BlogDate = DateTime.Now;
                var result = await _blogRepository.SaveBlogAsync(blog);

                if (result != null)
                {
                    TempData["Status"] = "Success";
                    TempData["Message"] = "Blog has been saved successfully.";
                    return RedirectToAction("Index");
                }

                TempData["Status"] = "Fail";
                TempData["Message"] = "Blog could not be saved.";
                return View(blog);
            }
            catch (Exception ex)
            {
                TempData["Status"] = "Error";
                TempData["Message"] = "An error occurred while saving the blog.";

                await _logEntryRepository.SaveLogEntry(new LogEntry
                {
                    LogDate = DateTime.Now,
                    TableName = "Blogs",
                    ActionType = "Blog->GetBlogById",
                    LogMessage = ex.Message,
                    LogLevel = ex.StackTrace,
                    UserName = user?.FirstName + " " + user?.LastName
                });

                return View(blog);
            }
        }


    }
}
