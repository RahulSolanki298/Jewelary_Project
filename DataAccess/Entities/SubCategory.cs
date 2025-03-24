using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    public class SubCategory
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Please enter your subcategory.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please select category.")]
        public int CategoryId { get; set; }
    }
}
