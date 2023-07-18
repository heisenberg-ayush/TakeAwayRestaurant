namespace PointOfSale.Models
{
    public class CustomerOrder
    {
        public int CustomerOrderID { get; set; }
        public int CustomerID { get; set; }
        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public int OrderType { get; set; }
        public int OnlinePartnerID { get; set; }
        public string PartnerOrderNumber { get; set; }
        public int AddressID { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string DeliveryTime { get; set; }
        public string DeliveryLocation { get; set; }
        public decimal TotalValue { get; set; }
        public DateTime OrderInstance { get; set; }
        public int IsDeliveryApplicable { get; set; }
        public int KitchenOrderTicketID { get; set; }
        public int SaleBillID { get; set; }
        public int IsCancel { get; set; }
        public int IsNoSpoon { get; set; }
        public int IsCustomerApplication { get; set; }
        public int PointsRedeemed { get; set; }
        public int PaymentMethod { get; set; }
        public int PointsEarned { get; set; }
        public decimal PointRedeemAmount { get; set; }
        public decimal SGST_Rate { get; set; }
        public decimal CGST_Rate { get; set; }
        public decimal SGST_Amount { get; set; }
        public decimal CGST_Amount { get; set; }
        public int OrderStatus { get; set; }
        public string Remarks { get; set; }
        public decimal NetPayable { get; set; }
        public decimal RoundOff { get; set; }
        public string CustomerName { get; set; }
        public string MobileNumber { get; set; }
        public int IsReturn { get; set; }
    }
}
