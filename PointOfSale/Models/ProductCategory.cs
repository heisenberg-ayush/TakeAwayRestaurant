using System.ComponentModel.DataAnnotations;

namespace PointOfSale.Models
{
    public class ProductCategory
    {
        [Key]
        public int ProductCategoryID { get; set; }
        public string ProductCategoryName { get; set; }
    }
}
