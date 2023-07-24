using Dapper;
using Microsoft.AspNetCore.Mvc;
using PointOfSale.Models;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PointOfSale.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerValidateController : ControllerBase
    {
        private readonly IConfiguration _config;
        public CustomerValidateController(IConfiguration config)
        {
            _config = config;
        }

        // POST: api/<CustomerValidateController>
        [HttpPost]
        public async Task<IActionResult> ValidateMobile(string customerMobile)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    await conn.OpenAsync();
                    var sql = @"SELECT [CustomerID]
                        FROM [Lazzatt].[dbo].[Customer]
                        WHERE CustomerMobile = @CustomerMobile";
                    var result = await conn.QuerySingleOrDefaultAsync<string>(sql, new { CustomerMobile = customerMobile });

                    if (result != null)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        return NotFound("Customer mobile number does not exist.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
