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
    public class CityAreaController : ControllerBase
    {
        private readonly IConfiguration _config;
        public CityAreaController(IConfiguration config)
        {
            _config = config;
        }

        // GET api/<CityAreaController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                using (var conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"SELECT [AreaName]
                          FROM [Lazzatt].[dbo].[CityArea]
                          WHERE CityAreaID = @ID";

                    var areaList = await conn.QueryFirstOrDefaultAsync<string>(sql, new { ID = id });

                    if (areaList == null)
                    {
                        return NotFound(); // Return 404 if area of customer is not found
                    }

                    return Ok(areaList);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately, log the error, or return a specific error response
                return StatusCode(500, "An error occurred while retrieving customer's area.");
            }
        }

        // POST api/<CityAreaController>
        [HttpPost]
        public async Task<IActionResult> Post(CityArea area)
        {
            try
            {
                using (var conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"INSERT INTO [Lazzatt].[dbo].[CityArea]
                                           ([AreaName]
                                           ,[PostalCodeStart]
                                           ,[PostalCodeEnd])
                                     VALUES
                                           (@AreaName, @PostalCodeStart, @PostalCodeEnd)";

                    var newArea = new CityArea()
                    {
                        AreaName = area.AreaName,
                        PostalCodeStart = area.PostalCodeStart,
                        PostalCodeEnd = area.PostalCodeEnd
                    };
                    var result = await conn.ExecuteAsync(sql, newArea);

                    return Ok("Data Inserted");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately, log the error, or return a specific error response
                return StatusCode(500, "An error occurred while inserting customer address data.");
            }
        }

        // DELETE api/<CityAreaController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Check if the Area exists with the given ID
                bool areaExists = await CheckIfAreaExistsAsync(id);

                if (!areaExists)
                {
                    return NotFound("Area not Found");
                }

                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"DELETE FROM [Lazzatt].[dbo].[CityArea]
                        WHERE CityAreaID = @CityAreaID";

                    await conn.ExecuteAsync(sql, new { CityAreaID = id });

                    return Ok("Area Deleted");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                string errorMessage = "An error occurred while deleting the area";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return StatusCode((int)statusCode, errorMessage);
            }
        }

        private async Task<bool> CheckIfAreaExistsAsync(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    // Prepare the SQL query to check if the area exists
                    var query = "SELECT COUNT(*) FROM CityArea WHERE CityAreaID = @ID";

                    // Execute the query and retrieve the count asynchronously using Dapper
                    int count = await conn.ExecuteScalarAsync<int>(query, new { ID = id });

                    // If the count is greater than 0, the area exists
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, log the error, and return false (assuming area doesn't exist)
                // For example:
                // logger.LogError(ex, "Error checking if customer type exists");
                return false;
            }
        }
    }
}
