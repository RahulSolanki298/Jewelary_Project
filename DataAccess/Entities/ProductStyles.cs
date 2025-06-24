using System;

namespace DataAccess.Entities
{
    public class ProductStyles
    {
        public int Id { get; set; }

        public string StyleName { get; set; }

        public string VenderId { get; set; }

        public int? CategoryId { get; set; }

        public string StyleImage { get; set; }

        public string CoverPageImage { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }

        public bool? IsActivated { get; set; } = false;

    }
}
