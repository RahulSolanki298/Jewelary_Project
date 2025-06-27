using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    public class HomePageSetting
    {
        public int Id { get; set; }
        public string Device { get; set; } //  Web,Android App
        public string PageName { get; set; } //  Web,Android App
        public string PageFrontImg { get; set; } //  Web,Android App
        public string CompanyLogo { get; set; }
        [NotMapped]
        public IFormFile CompanyLogoFile { get; set; }
        public bool isSetVideo { get; set; } // Company Video
        public string SetVideoPath { get; set; }
        [NotMapped]
        public IFormFile VideoFile { get; set; }
        public string VdoTitle { get; set; }
        public string VdoMetaUrl { get; set; }  //  Set Meta Page URL

        public bool isSetCompanySlider { get; set; } // Company Video
        public string SetSlider1Path { get; set; }
        [NotMapped]
        public IFormFile Image1Path { get; set; }
        public string Slider1Title { get; set; }
        public string Slider1MetaUrl { get; set; }  //  Set Meta Page URL
        public string SetSlider2Path { get; set; }
        [NotMapped]
        public IFormFile Image2Path { get; set; }
        public string Slider2Title { get; set; }
        public string Slider2MetaUrl { get; set; }  //  Set Meta Page URL
        public string SetSlider3Path { get; set; }
        [NotMapped]
        public IFormFile Image3Path { get; set; }
        public string Slider3Title { get; set; }
        public string Slider3MetaUrl { get; set; }  //  Set Meta Page URL

        public string DiamondIds { get; set; }
        public string CategoryIds { get; set; }
        public string StylesIds { get; set; }
        public string CollectionIds { get; set; }
        public string BlogIds { get; set; }
        public string Description { get; set; }

        [NotMapped]
        public List<Category> CategoryList { get; set; }

        [NotMapped]
        public List<ProductStyles> StyleList { get; set; }

        [NotMapped]
        public List<ProductCollections> CollectionList { get; set; }
    }
}
