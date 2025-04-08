namespace Models
{
    public class DiamondFilters
    {
        public int?[] Shapes { get; set; }

        public int?[] Colors { get; set; } 

        public string FromCarat { get; set; } 
        public string ToCarat { get; set; }

        public decimal? FromPrice { get; set; }
        public decimal? ToPrice { get; set; }

        public int[] Cuts { get; set; }

        public int[] Clarities { get; set; }

        public string FromRatio { get; set; }
        public string ToRatio { get; set; }

        public string FromTable { get; set; }
        public string ToTable { get; set; }

        public string FromDepth { get; set; }
        public string ToDepth { get; set; }

        public int?[] Polish { get; set; }

        public int?[] Fluor { get; set; }

        public int?[] Symmeties { get; set; }

    }
}
