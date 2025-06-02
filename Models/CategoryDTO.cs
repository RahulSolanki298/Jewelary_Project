namespace Models
{
    public class CategoryDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Prefix { get; set; }

        public string Title { get; set; }

        public string SEO_Title { get; set; }

        public string SEO_Url { get; set; }

        public int DisplayOrder { get; set; }

        public string CategoryImage { get; set; }

        public string ProductType { get; set; }

        public bool IsActivated { get; set; }
    }
}
