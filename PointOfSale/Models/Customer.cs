namespace PointOfSale.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobile { get; set; }
        public string GSTIN { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int EarnPoints { get; set; }
        public int RedeemPoints { get; set; }
        public int HasShoppingCartItems { get; set; }
        public int RequireReLogin { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime CannotLoginUntilDateUtc { get; set; }
        public int IsInactive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime LastActivityDate { get; set; }
        public string AdminComment { get; set; }
        public string AuthenticationType { get; set; }
        public string CustomerGender { get; set; }
        public DateTime CustomerDOB { get; set; }
        public DateTime CustomerAnniversary { get; set; }
        public int CustomerType { get; set; }
        public int FCMToken { get; set; }
        public int IsOnlineCustomer { get; set; }


    }
    public enum GenderEnum
    {
        Male,
        Female,
        Other
    }
}
