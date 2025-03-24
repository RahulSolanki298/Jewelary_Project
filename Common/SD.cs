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

        // Product Types
        public static string GoldProducts = "Gold";
        public static string DiamondProducts = "Diamond";
        public static string GDProducts = "GoldAndDiamond";

        public static string Activated = "Activated";
        public static string DeActived = "DeActived";
        public static string InProcess = "InProcess";

        // Diamond Property
        public static string Metal = "Metal";
        public static string Clarity = "Clarity";
        public static string Shape = "Shape";
        public static string Carat = "Carat";


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
        public static string Cancelled = "Cancelled";
        public static string Returned = "Returned";
        public static string Refunded = "Refunded";

        public static string BaseApiUrl = "https://localhost:4050"; // https://jswebapi.com


    }
}
