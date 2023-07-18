namespace PointOfSale.Models
{
    public class CustomerAddresses
    {
        public int AddressID { get; set; }
        public int CustomerID { get; set; }
        public string FriendlyName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PostalCode { get; set; }
        public int AreaID { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Landmark { get; set; }
        public int Discontinued { get; set; }
    }
}
