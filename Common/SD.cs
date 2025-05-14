namespace Common
{
    public class SD
    {
        /* Users*/
        public static string Admin = "Admin";
        public static string Supplier = "Supplier";
        public static string Employee = "Employee";

        /* Customers for b2c*/
        public static string Customer = "Customer";

        /* Customers for b2b*/
        public static string BusinessAccount = "BusinessAccount";


        public static string Requested = "Requested";

        public static string StartActivationProcess = "Start Activation Process"; // Pending Process

        public static string ReadyToPdp = "READY TO PDP";

        // Product Types
        public static string GoldProducts = "Gold";

        public static string DiamondProducts = "Diamond";

        public static string GDProducts = "GoldAndDiamond";

        public static string Activated = "Activated";

        public static string DeActived = "DeActived";

        public static string InProcess = "InProcess";

        // Jewelary Property
        public static string Metal = "Metal";


        // Diamond Property
        public static string Color = "Color";
        public static string Clarity = "Clarity";
        public static string Shape = "Shape";
        public static string Carat = "Carat";
        public static string Ratio = "Ratio";
        public static string Fluor = "Fluor";
        public static string Polish = "Polish";
        public static string Symmetry = "Symmetry";
        public static string Table = "Table";
        public static string CaratSize = "Center-Carat";//"CaratSize";
        public static string Depth = "Depth";
        public static string Lab = "Lab";
        public static string Cut = "Cut";
        public static string _TYPE = "TYPE";

        // Supplier Activation Status
        public static string Approve = "Approve";
        public static string DisApprove = "DisApprove";

        //  Order System
        public static string OrderRequested = "OrderRequested";
        public static string OrderAccepted = "OrderAccepted";
        public static string OrderRejected = "OrderRejected";
        public static string Processing = "Processing";
        public static string StartPackaging = "StartPackaging";
        public static string ReadyForShipment = "ReadyForShipment";
        public static string Shipped = "Shipped";
        public static string InTransit = "InTransit";
        public static string OutForDelivery = "OutForDelivery";
        public static string Delivered = "Delivered";
        public static string Complated = "Complated";
        public static string Cancelled = "Cancelled";
        public static string Returned = "Returned";
        public static string Refunded = "Refunded";

        public static string IGI = "IGI";
        public static string GIA = "GIA";


         public static string ShapeImgUrl = "https://localhost:44325";
         public static string ImgUrl = "https://localhost:44325";
         public static string BaseApiUrl = "https://localhost:44325"; // https://jswebapi.com

        //  public static string BaseApiUrl = "http://192.168.29.11:8082"; // https://jswebapi.com
        //  public static string BaseApiUrl = "http://192.168.100.153:8082"; // https://jswebapi.com

         //public static string ShapeImgUrl = "https://192.168.100.153:8092";
         //public static string ImgUrl = "http://192.168.100.153:8082";
         //public static string BaseApiUrl = "https://192.168.100.153:8082";

        //  local
        // public static string ShapeImgUrl = "http://180.211.116.206/jewelfacet";
        // public static string ImgUrl = "http://180.211.116.206/jewelfacet";
        // public static string BaseApiUrl = "http://180.211.116.206/jewelfacet";

        // production
        // public static string ShapeImgUrl = "http://192.168.100.153/jewelfacet";
        // public static string ImgUrl = "http://192.168.100.153/api.jewelfacet";
        // public static string BaseApiUrl = "http://192.168.100.153/api.jewelfacet";
    }
}