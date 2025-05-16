using System;

namespace Models
{
    public class DiamondFileUploadHistoryDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string UploadedBy { get; set; }

        public string UploadedPersonName { get; set; }

        public DateTime UploadedDate { get; set; }

        public int NoOfSuccess { get; set; }

        public int NoOfFailed { get; set; }

        public int IsSuccess { get; set; }

    }
}
