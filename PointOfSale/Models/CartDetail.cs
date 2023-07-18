namespace PointOfSale.Models
{
    public class CartDetail
    {
        public int CartDetailID { get; set; }
        public int CartID { get; set; }
        public int DetailSerial { get; set; }
        public int ProductID { get; set; }
        public decimal Quantity { get; set; }
        public int UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
    }
}
