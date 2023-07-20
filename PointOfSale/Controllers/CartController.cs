﻿using Dapper;
using Microsoft.AspNetCore.Mvc;
using PointOfSale.Models;
using System.Data.SqlClient;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PointOfSale.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IConfiguration _config;
        public CartController(IConfiguration config)
        {
            _config = config;
        }

        /*// GET: api/<CartController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<CartController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }*/

        // POST api/<CartController>
        // takes customer id
        [HttpPost]
        public async Task<IActionResult> Post(int id)
        {
            try
            {
                using (var conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    //First fill the Cart Database then Cart 
                    var sql = @"INSERT INTO [Lazzatt].[dbo].[Cart]
                                       ([CustomerID])
                                 VALUES
                                       (@CustomerID);
                                 SELECT CAST(SCOPE_IDENTITY() as int);";

                    await conn.ExecuteAsync(sql, new { CustomerID = id });

                    // Get CustomerID
                    int newId = await conn.QuerySingleAsync<int>(sql, new { CustomerID = id });

                    return Ok(newId);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately, log the error, or return a specific error response
                return StatusCode(500, "An error occurred while inserting customer data.");
            }
        }

        // PUT api/<CartController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CartController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Check if the cart detail exists with the given ID
                bool cartExists = await CheckIfCartExistsAsync(id);

                if (!cartExists)
                {
                    return NotFound("Cart not Found");
                }

                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"DELETE FROM [Lazzatt].[dbo].[Cart]
                        WHERE CartID = @CartID";

                    await conn.ExecuteAsync(sql, new { CartID = id });

                    return Ok("Cart Deleted");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                string errorMessage = "An error occurred while deleting the Cart";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return StatusCode((int)statusCode, errorMessage);
            }
        }

        private async Task<bool> CheckIfCartExistsAsync(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    // Prepare the SQL query to check if the cart exists
                    var query = "SELECT COUNT(*) FROM Cart WHERE CartID = @ID";

                    // Execute the query and retrieve the count asynchronously using Dapper
                    int count = await conn.ExecuteScalarAsync<int>(query, new { ID = id });

                    // If the count is greater than 0, the cart exists
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, log the error, and return false (assuming cart doesn't exist)
                // For example:
                // logger.LogError(ex, "Error checking if cart exists");
                return false;
            }
        }
    }
}
