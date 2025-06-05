using Business.Repository.IRepository;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ControlPanel.Controllers
{
    public class BlogController : Controller
    {
        public readonly IBlogRepository _blogRepository;
        public BlogController(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
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
        public async Task<IActionResult> SaveBlog(Blogs blog)
        {
            var result = await _blogRepository.SaveBlogAsync(blog);
            return View(result);
        }
    }
}
