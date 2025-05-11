namespace Models
{
    public class DiamondFilters
    {
        public string[] Shapes { get; set; }

        public string[] Colors { get; set; }

        public decimal? FromCarat { get; set; } 
        public decimal? ToCarat { get; set; }

        public decimal? FromPrice { get; set; }
        public decimal? ToPrice { get; set; }

        public string[] Cuts { get; set; }

        public string[] Clarities { get; set; }

        public decimal? FromRatio { get; set; }
        public decimal? ToRatio { get; set; }

        public decimal? FromTable { get; set; }
        public decimal? ToTable { get; set; }
        public decimal? FromDepth { get; set; }
        public decimal? ToDepth { get; set; }

        public string[] Polish { get; set; }

        public string[] Fluor { get; set; }

        public string[] Symmeties { get; set; }

        public string[] LabNames { get; set; }

    }
}
