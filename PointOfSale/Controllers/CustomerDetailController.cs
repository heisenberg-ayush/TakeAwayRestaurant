using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PointOfSale.Models;
using System.Data.SqlClient;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PointOfSale.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerDetailController : ControllerBase
    {
        private readonly IConfiguration _config;
        public CustomerDetailController(IConfiguration config)
        {
            _config = config;
        }

        // GET: api/<CustomerDetailController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var sql = @"SELECT [CustomerDetailID], [CustomerID], [RecordType], [Name],
                           [BirthDay], [BirthMonth], [AnniversaryDay], [AnniversaryMonth]
                    FROM [Lazzatt].[dbo].[CustomerDetail]";

                using (var conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var customerList = await conn.QueryAsync<CustomerDetail>(sql);
                    return Ok(customerList);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                string errorMessage = "An error occurred while retrieving the customer details";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return StatusCode((int)statusCode, errorMessage);
            }
        }

        // GET api/<CustomerDetailController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"SELECT [CustomerDetailID], [CustomerID], [RecordType], [Name],
                               [BirthDay], [BirthMonth], [AnniversaryDay], [AnniversaryMonth]
                        FROM [Lazzatt].[dbo].[CustomerDetail]
                        WHERE CustomerDetailID = @ID";

                    var customer = await conn.QuerySingleOrDefaultAsync<CustomerDetail>(sql, new { ID = id });

                    if (customer == null)
                    {
                        return NotFound();
                    }

                    return Ok(customer);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                string errorMessage = "An error occurred while retrieving the customer details";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return StatusCode((int)statusCode, errorMessage);
            }
        }

        // POST api/<CustomerDetailController>
        [HttpPost]
        public async Task<IActionResult> Post(CustomerDetail customerDetail)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"INSERT INTO [Lazzatt].[dbo].[CustomerDetail]
                            ([CustomerID]
                            ,[RecordType]
                            ,[Name]
                            ,[BirthDay]
                            ,[BirthMonth]
                            ,[AnniversaryDay]
                            ,[AnniversaryMonth])
                        VALUES
                            (@CustomerID, @RecordType, @Name, @BirthDay, @BirthMonth, @AnniversaryDay, @AnniversaryMonth)";

                    var newCustomer = new CustomerDetail()
                    {
                        CustomerID = customerDetail.CustomerID,
                        RecordType = customerDetail.RecordType,
                        Name = customerDetail.Name,
                        BirthDay = customerDetail.BirthDay,
                        BirthMonth = customerDetail.BirthMonth,
                        AnniversaryDay = customerDetail.AnniversaryDay,
                        AnniversaryMonth = customerDetail.AnniversaryMonth
                    };

                    var result = await conn.ExecuteAsync(sql, newCustomer);
                    return Ok("Data Inserted");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                string errorMessage = "An error occurred while inserting the customer details";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return StatusCode((int)statusCode, errorMessage);
            }
        }

        // PUT api/<CustomerDetailController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, CustomerDetail customerDetail)
        {
            try
            {
                // Check if the customer exists with the given ID
                bool customerExists = await CheckIfCustomerDetailExistsAsync(id);

                if (!customerExists)
                {
                    return NotFound("Customer not Found");
                }

                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"UPDATE [Lazzatt].[dbo].[CustomerDetail]
                        SET RecordType = @RecordType,
                            Name = @Name,
                            BirthDay = @BirthDay,
                            BirthMonth = @BirthMonth,
                            AnniversaryDay = @AnniversaryDay,
                            AnniversaryMonth = @AnniversaryMonth
                        WHERE CustomerDetailID = @CustomerDetailID";

                    var newCustomer = new CustomerDetail()
                    {
                        CustomerDetailID = id,
                        RecordType = customerDetail.RecordType,
                        Name = customerDetail.Name,
                        BirthDay = customerDetail.BirthDay,
                        BirthMonth = customerDetail.BirthMonth,
                        AnniversaryDay = customerDetail.AnniversaryDay,
                        AnniversaryMonth = customerDetail.AnniversaryMonth
                    };

                    var result = await conn.ExecuteAsync(sql, newCustomer);

                    return Ok("Updated");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                string errorMessage = "An error occurred while updating the customer details";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return StatusCode((int)statusCode, errorMessage);
            }
        }

        // DELETE api/<CustomerDetailController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Check if the customer exists with the given ID
                bool customerExists = await CheckIfCustomerDetailExistsAsync(id);

                if (!customerExists)
                {
                    return NotFound("Customer not Found");
                }

                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"DELETE FROM [Lazzatt].[dbo].[CustomerDetail]
                        WHERE CustomerDetailID = @CustomerDetailID";

                    var result = await conn.ExecuteAsync(sql, new { CustomerDetailID = id });

                    return Ok("Customer Deleted");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                string errorMessage = "An error occurred while deleting the customer details";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return StatusCode((int)statusCode, errorMessage);
            }
        }


        private async Task<bool> CheckIfCustomerDetailExistsAsync(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    // Prepare the SQL query to check if the customer exists
                    var query = "SELECT COUNT(*) FROM CustomerDetail WHERE CustomerDetailID = @ID";

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
}
