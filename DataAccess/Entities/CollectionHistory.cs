using System;

namespace DataAccess.Entities
{
    public class CollectionHistory
    {
        public int Id { get; set; }

        public string HistoryTitle { get; set; }

        public DateTime? CreatedData { get; set; }

        public string CreatedBy { get; set; }

        public bool IsCurrent { get; set; }
    }
}
