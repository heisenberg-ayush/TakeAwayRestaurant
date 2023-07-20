using Dapper;
using Microsoft.AspNetCore.Mvc;
using PointOfSale.Models;
using System.Data.SqlClient;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PointOfSale.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IConfiguration _config;
        public CustomerController(IConfiguration config)
        {
            _config = config;
        }

        // GET: api/<CustomerController>
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var sql = @"SELECT TOP (100) [CustomerID]
                              ,[CustomerName]
                              ,[CustomerMobile]
                              ,[GSTIN]
                              ,[Email]
                              ,[EarnPoints]
                              ,[RedeemPoints]
                              ,[HasShoppingCartItems]
                              ,[CustomerGender]
                              ,[CustomerDOB]
                          FROM [Lazzatt].[dbo].[Customer]";

            try
            {
                using (var conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var customerList = await conn.QueryAsync<Customer>(sql);
                    return Ok(customerList);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately, log the error, or return a specific error response
                return StatusCode(500, "An error occurred while retrieving customer data.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                using (var conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"SELECT [CustomerID]
                            ,[CustomerName]
                            ,[CustomerMobile]
                            ,[Email]
                        FROM [Lazzatt].[dbo].[Customer]
                        WHERE CustomerID = @ID";

                    var customer = await conn.QueryFirstOrDefaultAsync<CustomerResponse>(sql, new { ID = id });

                    if (customer == null)
                    {
                        return NotFound(); // Return 404 if customer is not found
                    }

                    return Ok(customer);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately, log the error, or return a specific error response
                return StatusCode(500, "An error occurred while retrieving customer data.");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Post(Customer customer)
        {
            try
            {
                using (var conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"INSERT INTO [Lazzatt].[dbo].[Customer]
                            ([CustomerName]
                            ,[CustomerMobile]
                            ,[GSTIN]
                            ,[Email]
                            ,[Password]
                            ,[HasShoppingCartItems]
                            ,[CustomerGender]
                            ,[CustomerDOB]
                            ,[CustomerAnniversary]
                            ,[CustomerType]
                            ,[FCMToken]
                            ,[IsOnlineCustomer])
                        VALUES
                            (@CustomerName, @CustomerMobile, @GSTIN, @Email, @Password, @HasShoppingCartItems,
                            @CustomerGender, @CustomerDOB, @CustomerAnniversary, @CustomerType, @FCMToken, @IsOnlineCustomer)";

                    var newCustomer = new Customer()
                    {
                        CustomerName = customer.CustomerName,
                        CustomerMobile = customer.CustomerMobile,
                        GSTIN = customer.GSTIN,
                        Email = customer.Email,
                        Password = customer.Password,
                        HasShoppingCartItems = customer.HasShoppingCartItems,
                        CustomerGender = customer.CustomerGender,
                        CustomerDOB = customer.CustomerDOB,
                        CustomerAnniversary = customer.CustomerAnniversary,
                        CustomerType = customer.CustomerType,
                        FCMToken = customer.FCMToken,
                        IsOnlineCustomer = customer.IsOnlineCustomer
                    };

                    var result = await conn.ExecuteAsync(sql, newCustomer);

                    return Ok("Data Inserted");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately, log the error, or return a specific error response
                return StatusCode(500, "An error occurred while inserting customer data.");
            }
        }


        // PUT api/<CustomerController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Customer customer)
        {
            // Check if the customer exists with the given ID
            bool customerExists = await CheckIfCustomerExistsAsync(id);

            if (!customerExists)
            {
                return NotFound("Customer not Found");
            }

            SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString());

            var sql = @"UPDATE [Lazzatt].[dbo].[Customer]
                               SET CustomerName = @CustomerName,
                                   CustomerMobile = @CustomerMobile,
                                   GSTIN = @GSTIN,
                                   Email = @Email,
                                   EarnPoints = @EarnPoints,
                                   RedeemPoints = @RedeemPoints,
                                   HasShoppingCartItems = @HasShoppingCartItems,
                                   RequireReLogin = @RequireReLogin,
                                   FailedLoginAttempts = @FailedLoginAttempts,
                                   CannotLoginUntilDateUtc = @CannotLoginUntilDateUtc,
                                   IsInactive = @IsInactive,
                                   CreatedOn = @CreatedOn,
                                   LastLoginDate = @LastLoginDate,
                                   LastActivityDate = @LastActivityDate,
                                   AdminComment = @AdminComment,
                                   AuthenticationType = @AuthenticationType,
                                   CustomerGender = @CustomerGender,
                                   CustomerDOB = @CustomerDOB,
                                   CustomerAnniversary = @CustomerAnniversary,
                                   CustomerType = @CustomerType,
                                   FCMToken = @FCMToken,
                                   IsOnlineCustomer = @IsOnlineCustomer
                            WHERE
                                CustomerID = @CustomerID";

            var newCustomer = new Customer()
            {
                CustomerID = id,
                CustomerName = customer.CustomerName,
                CustomerMobile = customer.CustomerMobile,
                GSTIN = customer.GSTIN,
                Email = customer.Email,
                EarnPoints = customer.EarnPoints,
                RedeemPoints = customer.RedeemPoints,
                HasShoppingCartItems = customer.HasShoppingCartItems,
                RequireReLogin = customer.RequireReLogin,
                FailedLoginAttempts = customer.FailedLoginAttempts,
                CannotLoginUntilDateUtc = customer.CannotLoginUntilDateUtc,
                IsInactive = customer.IsInactive,
                CreatedOn = customer.CreatedOn,
                LastLoginDate = customer.LastLoginDate,
                LastActivityDate = customer.LastActivityDate,
                AdminComment = customer.AdminComment,
                AuthenticationType = customer.AuthenticationType,
                CustomerGender = customer.CustomerGender,
                CustomerDOB = customer.CustomerDOB,
                CustomerAnniversary = customer.CustomerAnniversary,
                CustomerType = customer.CustomerType,
                FCMToken = customer.FCMToken,
                IsOnlineCustomer = customer.IsOnlineCustomer
            };
            try
            {
                await conn.ExecuteAsync(sql, newCustomer);
                return Ok("Updated");
            }
            catch (Exception ex)
            {
                // Handle the exception, log the error, and return an appropriate error response
                // For example:
                // logger.LogError(ex, "Error updating customer");
                return StatusCode(500, "An error occurred while updating the customer");
            }
        }

        // DELETE api/<CustomerController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Check if the customer exists with the given ID
                bool customerExists = await CheckIfCustomerExistsAsync(id);

                if (!customerExists)
                {
                    return NotFound("Customer not Found");
                }

                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"DELETE FROM [Lazzatt].[dbo].[Customer]
                        WHERE CustomerID = @CustomerID";

                    await conn.ExecuteAsync(sql, new { CustomerID = id });

                    return Ok("Customer Deleted");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                string errorMessage = "An error occurred while deleting the customer";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return StatusCode((int)statusCode, errorMessage);
            }
        }

        private async Task<bool> CheckIfCustomerExistsAsync(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    // Prepare the SQL query to check if the customer exists
                    var query = "SELECT COUNT(*) FROM Customer WHERE CustomerID = @ID";

                    // Execute the query and retrieve the count asynchronously using Dapper
                    int count = await conn.ExecuteScalarAsync<int>(query, new { ID = id });

                    // If the count is greater than 0, the customer exists
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, log the error, and return false (assuming customer doesn't exist)
                // For example:
                // logger.LogError(ex, "Error checking if customer type exists");
                return false;
            }
        }
    }

    public class CustomerResponse
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobile { get; set; }
        public string Email { get; set; }
    }
}
