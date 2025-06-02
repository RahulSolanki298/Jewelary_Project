using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    public class ProductProperty
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Please enter property name.")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string SymbolName { get; set; }

        public string Synonyms { get; set; }

        public string IconPath { get; set; }

        public int? ParentId { get; set; }

        public int DisplayOrder { get; set; }

        public bool? IsActive { get; set; }

    }
}
