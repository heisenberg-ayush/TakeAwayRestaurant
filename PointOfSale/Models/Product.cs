namespace PointOfSale.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int ProductTypeID { get; set; }
        public decimal Rate { get; set; }
        public int Discontinued { get; set; }
        public string ProductImage { get; set; }
        public int HSN_Code { get; set; }
        public int UOM { get; set; }
        public decimal PartnerRate { get; set; }
        public string Description { get; set; }
        public string ServeFor { get; set; }
        public int FoodprepareTime { get; set; }
        public int CuisineTime { get; set; }
        public int VeganType { get; set; }
        public bool IsVeg { get; set; }
        public int WebSiteDisPlayOrder { get; set; }
        public int OfferInCartToBuy { get; set; }
        public int ProductCategoryID { get; set; }
        public int IsOutOfStock { get; set; }
    }
}
