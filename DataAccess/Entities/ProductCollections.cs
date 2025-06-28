using System;
using System.Collections.Generic;

namespace DataAccess.Entities
{
    public class ProductCollections
    {
        public int Id { get; set; }

        public string CollectionName { get; set; }

        public string Descriptions { get; set; }

        public string CollectionImage { get; set; }

        public string CoverPageImage { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool? IsActivated { get; set; } = false;

        public string CoverPageTitle { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public bool IsDisplayHome { get; set; } = false;

        public string ProductType { get; set; }
    }
}
