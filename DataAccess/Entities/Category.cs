using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Please enter category name.")]
        public string Name { get; set; }

        public string Prefix { get; set; }

        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter seo title.")]
        public string SEO_Title { get; set; }

        [Required(ErrorMessage = "Please enter seo url.")]
        public string SEO_Url { get; set; }

        public int DisplayOrder { get; set; }

        public string CategoryImage { get; set; }

        public string ProductType { get; set; }

        public bool IsActivated { get; set; } = false;
    }
}
