using System;

namespace DataAccess.Entities
{
    public class Diamond
    {
        public int Id { get; set; }

        public string StoneId { get; set; }

        public string DNA { get; set; }

        public string Step { get; set; }

        public int? TypeId { get; set; }

        public string Measurement { get; set; }

        public string LabShape { get; set; }

        public decimal? RAP { get; set; }

        public decimal? RapAmount { get; set; }

        public string ReportType { get; set; }
        public string LotNo { get; set; }
        public DateTime? LabDate { get; set; }
        public int? LabId { get; set; }
        public string Quality { get; set; }
        public string LotType { get; set; }
        public int? CutId { get; set; }
        public int? PolishId { get; set; }
        public string Sku { get; set; }
        public int? SymmetryId { get; set; }
        public string Shade { get; set; }
        public string HA { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; } = 0;
        public int? ClarityId { get; set; }
        public string Clarity { get; set; }
        public decimal? Carat { get; set; }
        public int? ColorId { get; set; }
        public int? ShapeId { get; set; }
        public decimal? Table { get; set; }
        public decimal? Depth { get; set; }
        public decimal? Ratio { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public string Location { get; set; }  // City Name
        public string Status { get; set; } // Available, Consignment, Hold, In
        public DateTime? INWDate { get; set; }
        public DateTime? MarketDate { get; set; }
        public DateTime? ReportDate { get; set; }
        public string Type { get; set; }
        public string CertificateNo { get; set; }
        public string Certificate { get; set; }
        public string Stock { get; set; }
        public string CertiType { get; set; }
        public string DisplayOrder { get; set; }
        public string Flo { get; set; }

        // Advance Search
        public decimal? Dia { get; set; }
        public string TableWhite { get; set; } // N,PP,ORY
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
        public int? FluorId { get; set; }
        public string Milky { get; set; }
        public string Luster { get; set; }
        public string Graining { get; set; }
        public string DaysType { get; set; }
        public decimal? Discount { get; set; }
        public decimal? RatePct { get; set; }
        public decimal? Amount { get; set; }
        public string DiamondImagePath { get; set; }
        public string DiamondVideoPath { get; set; }
        public string Diam { get; set; }
        public string OLD_PID { get; set; }
        public string ORAP { get; set; }
        public string MfgRemark { get; set; }
        public int? PriceNameId { get; set; }
        public string MDisc { get; set; }
        public string MRate { get; set; }
        public string MAmt { get; set; }
        public string EyeClean { get; set; }
        public string StrLan { get; set; }
        public string LrHalf { get; set; }
        public string KeyToSymbol { get; set; }
        public string LabComment { get; set; }
        public string OpenTable { get; set; }
        public string OpenCrown { get; set; }
        public string OpenPavallion { get; set; }
        public string OpenGirdle { get; set; }

        public string NT_OR_INT { get; set; }

        public string Pav_Ex_Fac { get; set; }

        public string GirdleDesc { get; set; }

        public bool? IsActivated { get; set; } = false;
    }
}
