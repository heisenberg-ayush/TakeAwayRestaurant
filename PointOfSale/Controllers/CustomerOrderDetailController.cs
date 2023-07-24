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
    public class CustomerOrderDetailController : ControllerBase
    {
        private readonly IConfiguration _config;
        public CustomerOrderDetailController(IConfiguration config)
        {
            _config = config;
        }

        // GET api/<CustomerOrderDetailController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                using (var conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"SELECT [CustomerOrderDetailID]
                              ,[CustomerOrderID]
                              ,[DetailSerial]
                              ,[ProductID]
                              ,[Quantity]
                              ,[UnitPrice]
                              ,[Amount]
                              ,[Remarks]
                          FROM [Lazzatt].[dbo].[CustomerOrderDetail]
                          WHERE CustomerOrderID = @ID";

                    var customerOrderDetailList = await conn.QueryAsync<CustomerOrderDetail>(sql, new { ID = id });

                    if (customerOrderDetailList == null)
                    {
                        return NotFound(); // Return 404 if customerOrderDetail is not found
                    }

                    return Ok(customerOrderDetailList);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately, log the error, or return a specific error response
                return StatusCode(500, "An error occurred while retrieving customer order detail data.");
            }
        }

        // POST api/<CustomerOrderDetailController>
        [HttpPost]
        // Gets CustomerORder ID
        public async Task<IActionResult> Post(int CartID, int CustomerOrderID)
        {
            try
            {
                using (var conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var getCartSql = @"SELECT [DetailSerial]
                              ,[ProductID]
                              ,[Quantity]
                              ,[UnitPrice]
                              ,[Amount]
                              ,[Remarks]
                          FROM [Lazzatt].[dbo].[CartDetail]
                          WHERE CartID = @ID";

                    var cartDetailList = await conn.QueryAsync<CartDetail>(getCartSql, new { ID = CartID });


                    var insertSql = @"INSERT INTO [Lazzatt].[dbo].[CustomerOrderDetail]
                                       ([CustomerOrderID]
                                       ,[DetailSerial]
                                       ,[ProductID]
                                       ,[Quantity]
                                       ,[UnitPrice]
                                       ,[Amount]
                                       ,[Remarks])
                                 VALUES
                                       (@CustomerOrderID, @CartID, @DetailSerial, @ProductID, @Quantity, @UnitPrice, @Amount, @Remarks)";


                    foreach (var cartDetail in cartDetailList)
                    {
                        // Create a new parameter object with values from cartDetail
                        var parameters = new CustomerOrderDetail()
                        {
                            CustomerOrderID = CustomerOrderID,
                            DetailSerial = cartDetail.DetailSerial,
                            ProductID = cartDetail.ProductID,
                            Quantity = cartDetail.Quantity,
                            UnitPrice = cartDetail.UnitPrice,
                            Amount = cartDetail.Amount,
                            Remarks = cartDetail.Remarks
                        };

                        // Execute the insert query for each CartDetail
                        await conn.ExecuteAsync(insertSql, parameters);
                    }

                    return Ok("Data Inserted");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately, log the error, or return a specific error response
                return StatusCode(500, "An error occurred while inserting customer order data.");
            }
        }

        // DELETE api/<CustomerOrderDetailController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Check if the customerOrder detail exists with the given ID
                bool customerOrderDetailExists = await CheckIfCustomerOrderDetailExistsAsync(id);

                if (!customerOrderDetailExists)
                {
                    return NotFound("CustomerOrder Detail not Found");
                }

                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"DELETE FROM [Lazzatt].[dbo].[CustomerOrderDetail]
                        WHERE CustomerOrderDetailID = @CustomerOrderDetailID";

                    await conn.ExecuteAsync(sql, new { CustomerOrderDetailID = id });

                    return Ok("Customer Order Detail Deleted");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                string errorMessage = "An error occurred while deleting the CustomerOrder Detail";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return StatusCode((int)statusCode, errorMessage);
            }
        }

        private async Task<bool> CheckIfCustomerOrderDetailExistsAsync(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    // Prepare the SQL query to check if the customerOrder detail exists
                    var query = "SELECT COUNT(*) FROM CustomerOrderDetail WHERE CustomerOrderDetailID = @ID";

                    // Execute the query and retrieve the count asynchronously using Dapper
                    int count = await conn.ExecuteScalarAsync<int>(query, new { ID = id });

                    // If the count is greater than 0, the customerOrder detail exists
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, log the error, and return false (assuming customerOrder detail doesn't exist)
                // For example:
                // logger.LogError(ex, "Error checking if customerOrder detail exists");
                return false;
            }
        }
    }
}
