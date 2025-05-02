namespace DataAccess.Entities
{
    public class UserAddress
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string CityName { get; set; }

        public string StateName { get; set; }

        public string Pincode { get; set; }

        public string Location { get; set; }

        public bool IsDefaultAddress { get; set; } = false;
    }
}
