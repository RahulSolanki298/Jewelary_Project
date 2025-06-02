using System.ComponentModel.DataAnnotations;
namespace DataAccess.Entities
{
    public class DiamondProperty
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Please enter diamond property")]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public int? ParentId { get; set; }

        [StringLength(255)]
        [Display(Name = "Icon Path")]
        public string IconPath { get; set; }

        [StringLength(100)]
        [Display(Name = "Symbol Name")]
        public string SymbolName { get; set; }

        [StringLength(50)]
        [Display(Name = "Color Type")]
        public string ColorType { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Display Order must be a non-negative number")]
        [Display(Name = "Display Order")]
        public int DispOrder { get; set; }

        [Display(Name = "Is Activated")]
        public bool IsActivated { get; set; }
    }

}
