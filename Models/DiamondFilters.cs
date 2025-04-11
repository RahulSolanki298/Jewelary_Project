namespace Models
{
    public class DiamondFilters
    {
        public int?[] Shapes { get; set; }

        //public int?[] Colors { get; set; } 
        public string[] Colors { get; set; }

        public decimal? FromCarat { get; set; } 
        public decimal? ToCarat { get; set; }

        public decimal? FromPrice { get; set; }
        public decimal? ToPrice { get; set; }

        public int[] Cuts { get; set; }

        public int[] Clarities { get; set; }

        public decimal? FromRatio { get; set; }
        public decimal? ToRatio { get; set; }

        public decimal? FromTable { get; set; }
        public decimal? ToTable { get; set; }
        public decimal? FromDepth { get; set; }
        public decimal? ToDepth { get; set; }

        public int?[] Polish { get; set; }

        public int?[] Fluor { get; set; }

        public int?[] Symmeties { get; set; }

    }
}
