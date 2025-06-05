using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    public class Blogs
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string BlogCategory { get; set; }

        public string ShortDescription { get; set; }

        public string Description { get; set; }

        public string BlogImage { get; set; }

        public DateTime? BlogDate { get; set; }

        public string CreatedBy { get; set; }

        [NotMapped] // Prevent EF from mapping this to DB
        public IFormFile ImageFile { get; set; }

        public bool IsActive { get; set; }

    }
}
