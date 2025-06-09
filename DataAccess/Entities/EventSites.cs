namespace DataAccess.Entities
{
    public class EventSites
    {
        public int Id { get; set; }

        public string  EventName { get; set; }

        public string Description { get; set; }

        public string ProfileImage { get; set; }

        public string DiscountOn { get; set; }  //  Product or All Products

        public bool DiscountOnDiamond { get; set; }
        
        public string SelectedDiamonds { get; set; }

        public bool DiscountOnJewellery { get; set; }
        
        public string SelectedProducts { get; set; }

        public string Discount { get; set; }

        public string  StartDate { get; set; }

        public string  EndDate { get; set; }

        public bool IsActive { get; set; }
    }
}
