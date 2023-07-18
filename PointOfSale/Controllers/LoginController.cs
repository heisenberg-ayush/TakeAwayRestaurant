using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PointOfSale.Models;
using System.Data;
using System.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PointOfSale.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult<string>> Login([FromBody] Operator request)
        {
            // Retrieve the username and password from the request
            var name = request.OperatorName;
            var password = request.Password;

            // Authenticate the user based on the provided credentials
            if (await IsValidCredentials(name, password))
            {
                // Return the username to the frontend
                return Ok(name);
            }

            // Authentication failed
            return Unauthorized("Invalid credentials");
        }

        private async Task<bool> IsValidCredentials(string name, string password)
        {
            try
            {
                // Establish a connection to the database
                using (var connection = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString()))
                {
                    // Prepare the SQL query to retrieve the user's credentials from the database
                    var query = "SELECT COUNT(*) FROM Operator WHERE OperatorName = @OperatorName AND Password = @Password";

                    // Execute the query and retrieve the count of matching rows
                    int count = await connection.ExecuteScalarAsync<int>(query, new { OperatorName = name, Password = password });

                    // Check if the count is greater than 0
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and return false for invalid credentials
                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return false;
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OperatorResponse>>> GetOperators()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString()))
                {
                    await conn.OpenAsync();

                    var query = "SELECT OperatorName, OperatorID FROM Operator";
                    var userList = await conn.QueryAsync<OperatorResponse>(query);

                    return Ok(userList);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, log the error, or provide a specific error message
                // based on your application's requirements
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving operators");
            }
        }
    }
    public class OperatorResponse
    {
        public int OperatorID { get; set; }
        public string OperatorName { get; set; }
    }
}
