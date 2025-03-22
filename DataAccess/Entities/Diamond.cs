using System;

namespace DataAccess.Entities
{
    public class Diamond
    {
        public int Id { get; set; }
        public string ReportType { get; set; }
        public string LotNo { get; set; }
        public DateTime? LabDate { get; set; }
        public string Quality { get; set; }
        public string LotType { get; set; }
        public string Cut { get; set; }
        public string Polish { get; set; }
        public string Sku { get; set; }  // OTN23U01-D300R0B 
        public string Sym { get; set; }
        public string Shade { get; set; }
        public string HA { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; } = 0;
        public string Carat { get; set; }
        public string Clarity { get; set; }
        public string Color { get; set; }
        public string Shape { get; set; }
        public decimal? Table { get; set; }
        public decimal? Depth { get; set; }
        public decimal? Ratio { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public string Location{ get; set; }  // City Name
        public string Status{ get; set; } // Available, Consignment, Hold, In
        public DateTime? INWDate { get; set; }
        public DateTime? MarketDate { get; set; }
        public DateTime? ReportDate { get; set; }
        public string Type { get; set; }
        public string CertificateNo { get; set; }
        public string Stock { get; set; }
        public string CertiType { get; set; }
        public string DisplayOrder { get; set; }

        // Advance Search
        public decimal? Dia { get; set; }
        public string  TableWhite { get; set; } // N,PP,ORY
        public string SideWhite { get; set; } // N,PP,ORY
        public string TableBlack { get; set; }
        public string SideBlack { get; set; } // N,BPP,MIN,MED
        public string PavOpen { get; set; } // N,SML,MED,HV
        public decimal? GirdleOpen { get; set; }
        public decimal? CAngle { get; set; }
        public decimal? PAngle { get; set; }
        public decimal? PHt { get; set; }
        public decimal? CHt { get; set; }
        public string Girdle { get; set; }
        public string CrownExFac { get; set; }
        public string PavExFac { get; set; }
        public string TableSpot { get; set; }
        public string SideSpot { get; set; }
        public string NT_INT { get; set; }
        public string Culet { get; set; }
        public string Flor { get; set; }
        public string Milky { get; set; }
        public string Luster { get; set; }
        public string Graining { get; set; }
        public string DaysType { get; set; }
        public decimal? Discount { get; set; }
        public decimal? RatePct { get; set; }
        public decimal? Amount { get; set; }

        public bool? IsActivated { get; set; } = false;
    }
}
