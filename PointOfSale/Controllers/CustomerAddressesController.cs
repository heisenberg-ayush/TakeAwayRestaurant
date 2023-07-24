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
    public class CustomerAddressesController : ControllerBase
    {
        private readonly IConfiguration _config;
        public CustomerAddressesController(IConfiguration config)
        {
            _config = config;
        }

        // GET: api/<CustomerAddressesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var sql = @"SELECT [AddressID]
                            ,[CustomerID]
                            ,[FriendlyName]
                            ,[Address1]
                            ,[Address2]
                            ,[PostalCode]
                            ,[AreaID]
                            ,[Latitude]
                            ,[Longitude]
                            ,[Landmark]
                            ,[Discontinued]
                        FROM [Lazzatt].[dbo].[CustomerAddresses]";

            try
            {
                using (var conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var addressesList = await conn.QueryAsync<CustomerAddresses>(sql);
                    return Ok(addressesList);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately, log the error, or return a specific error response
                return StatusCode(500, "An error occurred while retrieving address data.");
            }
        }

        // GET api/<CustomerAddressesController>/5
        [HttpGet("{id}")]
        // this takes customer id
        public async Task<IActionResult> GetAddressOfCustomer(int id)
        {
            try
            {
                using (var conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"SELECT [AddressID]
                              ,[FriendlyName]
                              ,[CustomerID]
                              ,[Address1]
                              ,[Address2]
                              ,[PostalCode]
                              ,[AreaID]
                              ,[Latitude]
                              ,[Longitude]
                              ,[Landmark]
                          FROM [Lazzatt].[dbo].[CustomerAddresses]
                          WHERE CustomerID = @ID";

                    var addrList = await conn.QueryAsync<CustomerAddresses>(sql, new { ID = id });

                    if (addrList == null)
                    {
                        return NotFound(); // Return 404 if address of customer is not found
                    }

                    return Ok(addrList);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately, log the error, or return a specific error response
                return StatusCode(500, "An error occurred while retrieving customer's address.");
            }
        }

        // POST api/<CustomerAddressesController>
        // needs Customer id 
        [HttpPost("{id}")]
        public async Task<IActionResult> Post(int id, CustomerAddresses addr)
        {
            try
            {
                using (var conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"INSERT INTO [Lazzatt].[dbo].[CustomerAddresses]
                                           ([CustomerID]
                                           ,[FriendlyName]
                                           ,[Address1]
                                           ,[Address2]
                                           ,[PostalCode]
                                           ,[AreaID]
                                           ,[Latitude]
                                           ,[Longitude]
                                           ,[Landmark]
                                           ,[Discontinued])
                                     VALUES
                                           (@CustomerID, @FriendlyName, @Address1, @Address2, @PostalCode, @AreaID, @Latitude, @Longitude, @Landmark, @Discontinued)";
                    
                    var newAddr = new CustomerAddresses()
                    {
                        CustomerID = id,
                        FriendlyName = addr.FriendlyName,
                        Address1 = addr.Address1,
                        Address2 = addr.Address2,
                        PostalCode = addr.PostalCode,
                        AreaID = addr.AreaID,
                        Latitude = addr.Latitude,
                        Longitude = addr.Longitude,
                        Landmark = addr.Landmark,
                        Discontinued = addr.Discontinued
                    };
                    var result = await conn.ExecuteAsync(sql, newAddr);

                    return Ok("Data Inserted");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately, log the error, or return a specific error response
                return StatusCode(500, "An error occurred while inserting customer address data.");
            }
        }

        // PUT api/<CustomerAddressesController>/5
        // takes address id as parameter
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, CustomerAddresses addr)
        {
            // Check if the customer address exists with the given ID
            bool addrExists = await CheckIfCustomerAddressExistsAsync(id);

            if (!addrExists)
            {
                return NotFound("Customer address not Found");
            }

            SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString());

            var sql = @"UPDATE [Lazzatt].[dbo].[CustomerAddresses]
                               SET FriendlyName = @FriendlyName,
                                   Address1 = @Address1,
                                   Address2 = @Address2,
                                   PostalCode = @PostalCode,
                                   AreaID = @AreaID,
                                   Latitude = @Latitude,
                                   Longitude = @Longitude,
                                   Landmark = @Landmark,
                                   Discontinued = @Discontinued
                            WHERE
                                AddressID = @AddressID";

            var newAddr = new CustomerAddresses()
            {
                AddressID = id,
                FriendlyName = addr.FriendlyName,
                Address1 = addr.Address1,
                Address2 = addr.Address2,
                PostalCode = addr.PostalCode,
                AreaID = addr.AreaID,
                Latitude = addr.Latitude,
                Longitude = addr.Longitude,
                Landmark = addr.Landmark,
                Discontinued = addr.Discontinued
            };

            try
            {
                await conn.ExecuteAsync(sql, newAddr);
                return Ok("Updated");
            }
            catch (Exception ex)
            {
                // Handle the exception, log the error, and return an appropriate error response
                // For example:
                // logger.LogError(ex, "Error updating customer");
                return StatusCode(500, "An error occurred while updating the customer address");
            }
        }

        // DELETE api/<CustomerAddressesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Check if the customer exists with the given ID
                bool addrExists = await CheckIfCustomerAddressExistsAsync(id);

                if (!addrExists)
                {
                    return NotFound("Customer Address not Found");
                }

                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"DELETE FROM [Lazzatt].[dbo].[CustomerAddresses]
                        WHERE AddressID = @AddressID";

                    await conn.ExecuteAsync(sql, new { AddressID = id });

                    return Ok("Customer Address Deleted");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                string errorMessage = "An error occurred while deleting the customer address";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return StatusCode((int)statusCode, errorMessage);
            }
        }

        private async Task<bool> CheckIfCustomerAddressExistsAsync(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    // Prepare the SQL query to check if the customer addr exists
                    var query = "SELECT COUNT(*) FROM CustomerAddresses WHERE AddressID = @ID";

                    // Execute the query and retrieve the count asynchronously using Dapper
                    int count = await conn.ExecuteScalarAsync<int>(query, new { ID = id });

                    // If the count is greater than 0, the customer address exists
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, log the error, and return false (assuming customer address doesn't exist)
                // For example:
                // logger.LogError(ex, "Error checking if customer addr exists");
                return false;
            }
        }
    }
}
