namespace PointOfSale.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int ProductTypeID { get; set; }
        public decimal rate { get; set; }
        public string ProductImage { get; set; }
    }
}
