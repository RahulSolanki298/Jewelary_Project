using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    public class HomePageSetting
    {
        public int Id { get; set; }

        public string Device { get; set; } //  Web,Android App

        public string CompanyLogo { get; set; }

        [NotMapped]
        public IFormFile CompanyLogoFile { get; set; }

        public bool isSetVideo { get; set; } // Company Video
        public string SetVideoPath { get; set; }

        [NotMapped]
        public IFormFile VideoFile { get; set; }

        public bool isSetCompanySlider { get; set; } // Company Video
        public string SetSlider1Path { get; set; }
        [NotMapped]
        public IFormFile Image1Path { get; set; }
        
        public string SetSlider2Path { get; set; }
        [NotMapped]
        public IFormFile Image2Path { get; set; }
        
        public string SetSlider3Path { get; set; }
        [NotMapped]
        public IFormFile Image3Path { get; set; }
    }
}
