namespace Models
{
    public class ProductFilters
    {
        public string[] Metals { get; set; }


        public string[] Shapes { get; set; }

        public string[] category { get; set; }

        //public string[] subCategory { get; set; }

        public decimal? FromPrice { get; set; }

        public decimal? ToPrice { get; set; }

        public decimal? FromCarat { get; set; }

        public decimal? ToCarat { get; set; }


    }
}
