using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class ProductStyleDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Style Name is required")]
        [StringLength(100)]
        public string StyleName { get; set; }

        public string VenderName { get; set; }

        public string VenderId { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }

        public bool? IsActivated { get; set; } = false;

        public string StyleImage { get; set; }

        public IFormFile ImageFile { get; set; }

        public string CoverPageImage { get; set; }

        public IFormFile CoverPageFile { get; set; }

        public int? CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public string CoverPageTitle { get; set; }
    }

}
