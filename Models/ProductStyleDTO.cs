using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class ProductStyleDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Style Name is required")]
        [StringLength(100)]
        public string StyleName { get; set; }

        [Required(ErrorMessage = "Vendor Name is required")]
        public string VenderName { get; set; }

        public string VenderId { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }

        public bool? IsActivated { get; set; } = false;

        public string StyleImage { get; set; }

    }

}
