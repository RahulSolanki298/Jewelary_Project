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
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public bool? IsActivated { get; set; } = false;
        public virtual ICollection<Product> Products { get; set; }

    }
}
