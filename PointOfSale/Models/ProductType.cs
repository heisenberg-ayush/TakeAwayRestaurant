namespace PointOfSale.Models
{
    public class ProductType
    {
        public int ProductTypeID { get; set; }
        public string ProductTypeName { get; set; }
        public string Description { get; set; }
        public int DontShowOnSite { get; set; }
        public int WebSiteDisPlayOrder { get; set; }
        public string ProducTypeImage { get; set; }
        public string OfferTitle { get; set; }
        public int OfferPercent { get; set; }
        public int ProductCategoryID { get; set; }
    }
}
