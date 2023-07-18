namespace PointOfSale.Models
{
    public class CustomerOrderDetail
    {
        public int CustomerOrderDetailID { get; set; }
        public int CustomerOrderID { get; set; }
        public int DetailSerial { get; set; }
        public int ProductID { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
    }
}
