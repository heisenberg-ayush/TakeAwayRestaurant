using System.ComponentModel.DataAnnotations;

namespace PointOfSale.Models
{
    public class Registration
    {
        public int OperatorID { get; set; }
        public string OperatorName { get; set; }
        public int MobileNumber { get; set; }
        public string Password { get; set; }
        public int OperatorType { get; set; }
        public int Discontinued { get; set; }
    }
}
