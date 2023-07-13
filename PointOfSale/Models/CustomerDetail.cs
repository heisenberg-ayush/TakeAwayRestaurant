namespace PointOfSale.Models
{
    public class CustomerDetail
    {
        public int CustomerDetailID { get; set; }
        public int CustomerID { get; set; }
        public int RecordType { get; set; }
        public string Name { get; set; }
        public int BirthDay { get; set; }
        public int BirthMonth { get; set; }
        public int AnniversaryDay { get; set; }
        public int AnniversaryMonth { get; set; }
    }
}
