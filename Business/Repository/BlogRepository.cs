using Business.Repository.IRepository;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Repository
{
    public class BlogRepository : IBlogRepository
    {
        private readonly ApplicationDBContext _context;
        public BlogRepository(ApplicationDBContext context) => _context = context;
        public async Task<Blogs> GetBlogById(int id) => await _context.Blogs.FindAsync(id);
        public async Task<List<Blogs>> GetBlogList() => await _context.Blogs.ToListAsync();
        public async Task<Blogs> SaveBlogAsync(Blogs blogs)
        {
            try
            {
                if (blogs != null && blogs.Id > 0)
                {
                    var data = await _context.Blogs.FindAsync(blogs.Id);
                    data.Title = blogs.Title;
                    data.Description = blogs.Description;
                    data.BlogImage = blogs.BlogImage;
                    
                    _context.Blogs.Update(data);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    await _context.Blogs.AddAsync(blogs);
                    await _context.SaveChangesAsync();
                }
                var result = await _context.Blogs.FindAsync(blogs.Id);
                return result;
            }
            catch (Exception ex)
            {
                await _context.LogEntries.AddAsync(new LogEntry
                {
                    LogLevel = ex.StackTrace,
                    LogDate = DateTime.Now,
                    LogMessage = ex.Message,
                    TableName = "Blogs",
                    ActionType = "Blog/AddBlogAsync",
                });
                await _context.SaveChangesAsync();

                return new Blogs();
            }

        }

        public async Task<bool> RemoveBlog(int id)
        {
            try
            {
                var blog = await _context.Blogs.FindAsync(id);
                if (blog != null)
                {
                    _context.Blogs.Remove(blog);
                    await _context.SaveChangesAsync();

                    return true;
                }

                return false;

            }
            catch (Exception ex)
            {
                await _context.LogEntries.AddAsync(new LogEntry
                {
                    LogLevel = ex.StackTrace,
                    LogDate = DateTime.Now,
                    LogMessage = ex.Message,
                    TableName = "Blogs",
                    ActionType = "Blog/DeleteBlog",
                });
                await _context.SaveChangesAsync();

                return false;
            }
        }
    }
}
