using System.ComponentModel.DataAnnotations;

namespace PointOfSale.Models
{
    public class Registration
    {
        [Key]
        public int OperatorID { get; set; }
        public string OperatorName { get; set; }
        public string Password { get; set; }
    }
}
