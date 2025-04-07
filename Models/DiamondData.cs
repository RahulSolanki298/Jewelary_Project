namespace Models
{
        public class DiamondData
        {
            public int Id { get; set; }
            public string StoneId { get; set; }
            public string DNA { get; set; }
            public string Step { get; set; }
            public int? TypeId { get; set; }
            public string TypeName { get; set; }
            public string Measurement { get; set; }
            public string LabShape { get; set; }
            public int? LabId { get; set; }
            public string LabName { get; set; }
            public int? ShapeId { get; set; }
            public string ShapeName { get; set; }
            public decimal? RAP { get; set; }
            public decimal? RapAmount { get; set; }
            public int? CaratId { get; set; }
            public string CaratName { get; set; }
            public int? ClarityId { get; set; }
            public string ClarityName { get; set; }
            public int? ColorId { get; set; }
            public string ColorName { get; set; }
            public int? CutId { get; set; }
            public string CutName { get; set; }
            public int? PolishId { get; set; }
            public string PolishName { get; set; }
            public int? SymmetryId { get; set; }
            public string SymmetyName { get; set; }
            public int? FluorId { get; set; }
            public string FluorName { get; set; }
            public decimal? Price { get; set; }
            public decimal? Table { get; set; }
            public decimal? Depth { get; set; }
            public int? RatioId { get; set; }
            public string RatioName { get; set; }
            public decimal? Quantity { get; set; } = 0;
            public string Shade { get; set; }
            public string Certificate { get; set; }
            public decimal? Discount { get; set; }
            public decimal? RatePct { get; set; }
            public decimal? Amount { get; set; }
            public string DiamondImagePath { get; set; }
            public string DiamondVideoPath { get; set; }
            public string IconPath { get; set; }
            public bool? IsActivated { get; set; } = false;
        }
}
