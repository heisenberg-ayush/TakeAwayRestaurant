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
    public class CartDetailController : ControllerBase
    { 
        private readonly IConfiguration _config;
        public CartDetailController(IConfiguration config)
        {
            _config = config;
        }

        // GET api/<CartDetailController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                using (var conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"SELECT [CartDetailID]
                              ,[DetailSerial]
                              ,[ProductID]
                              ,[Quantity]
                              ,[UnitPrice]
                              ,[Amount]
                              ,[Remarks]
                          FROM [Lazzatt].[dbo].[CartDetail]
                          WHERE CartID = @ID";

                    var cartDetailList = await conn.QueryAsync<CartDetail>(sql, new { ID = id });

                    if (cartDetailList == null)
                    {
                        return NotFound(); // Return 404 if cart is not found
                    }

                    return Ok(cartDetailList);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately, log the error, or return a specific error response
                return StatusCode(500, "An error occurred while retrieving cart data.");
            }
        }
        
        // POST api/<CartDetailController>
        // Takes CartID from Frontend
        [HttpPost]
        public async Task<IActionResult> Post(int id, CartDetail cart)
        {
            try
            {
                using (var conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"INSERT INTO [Lazzatt].[dbo].[CartDetail]
                                       ([CartID]
                                       ,[DetailSerial]
                                       ,[ProductID]
                                       ,[Quantity]
                                       ,[UnitPrice]
                                       ,[Amount]
                                       ,[Remarks])
                                 VALUES
                                       (@CartID, @DetailSerial, @ProductID, @Quantity, @UnitPrice, @Amount, @Remarks);";

                    var newCart = new CartDetail()
                    {
                        CartID = id,
                        DetailSerial = cart.DetailSerial,
                        ProductID = cart.ProductID,
                        Quantity = cart.Quantity,
                        UnitPrice = cart.UnitPrice,
                        Amount = cart.Amount,
                        Remarks = cart.Remarks
                    };

                    await conn.ExecuteAsync(sql, newCart);

                    return Ok("Data Inserted");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately, log the error, or return a specific error response
                return StatusCode(500, "An error occurred while inserting customer data.");
            }
        }

        // DELETE api/<CartDetailController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Check if the cart detail exists with the given ID
                bool cartDetailExists = await CheckIfCartDetailExistsAsync(id);

                if (!cartDetailExists)
                {
                    return NotFound("Cart Detail not Found");
                }

                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"DELETE FROM [Lazzatt].[dbo].[CartDetail]
                        WHERE CartDetailID = @CartDetailID";

                    await conn.ExecuteAsync(sql, new { CartDetailID = id });

                    return Ok("Cart Detail Deleted");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                string errorMessage = "An error occurred while deleting the Cart Detail";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return StatusCode((int)statusCode, errorMessage);
            }
        }

        private async Task<bool> CheckIfCartDetailExistsAsync(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    // Prepare the SQL query to check if the cart detail exists
                    var query = "SELECT COUNT(*) FROM CartDetail WHERE CartDetailID = @ID";

                    // Execute the query and retrieve the count asynchronously using Dapper
                    int count = await conn.ExecuteScalarAsync<int>(query, new { ID = id });

                    // If the count is greater than 0, the cart detail exists
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, log the error, and return false (assuming cart detail doesn't exist)
                // For example:
                // logger.LogError(ex, "Error checking if cart detail exists");
                return false;
            }
        }
    }
}
