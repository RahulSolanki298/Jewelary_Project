using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    public class ProductCollectionItems
    {
        public int Id { get; set; }

        public int CollectionId { get; set; }

        public string ProductId { get; set; }

        public string UserId { get; set; }

        public bool IsActive { get; set; }

        public bool IsHomePage { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int Index { get; set; }

    }
}
