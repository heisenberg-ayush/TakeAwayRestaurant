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
        public ActionResult<string> Login([FromBody] Operator request)
        {
            // Retrieve the username and password from the request
            var name = request.OperatorName;
            var password = request.Password;

            // Authenticate the user based on the provided credentials
            if (IsValidCredentials(name, password))
            {
                // Return the username to the frontend
                return Ok(name);
            }

            // Authentication failed
            return Unauthorized("null");
        }

        private bool IsValidCredentials(string name, string password)
        {
            // Establish a connection to the database
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString()))
            {
                connection.Open();

                // Prepare the SQL query to retrieve the user's credentials from the database
                var query = "SELECT OperatorName FROM Operator WHERE OperatorName = @OperatorName AND Password = @Password";

                using (var command = new SqlCommand(query, connection))
                {
                    // Set the parameters for the query
                    command.Parameters.AddWithValue("@OperatorName", name);
                    command.Parameters.AddWithValue("@Password", password);

                    // Execute the query and check if any rows are returned
                    using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (reader.Read())
                        {
                            // Valid credentials
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            // Invalid credentials
            return false;
        }


        [HttpGet]
        public ActionResult<IEnumerable<string>> GetOperators()
        {
            // Retrieve all registered users from the database
            var users = RegisteredOperator();

            return Ok(users);
        }

        private IEnumerable<OperatorResponse> RegisteredOperator()
        {
            var userList = new List<OperatorResponse>();

            // Establish a connection to the database
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString()))
            {
                conn.Open();

                // Prepare the SQL query to retrieve all registered users
                var query = "SELECT OperatorName, OperatorID FROM Operator";

                using (var command = new SqlCommand(query, conn))
                {
                    // Execute the query and retrieve the usernames
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Add each username to the list
                            OperatorResponse op = new OperatorResponse
                            {
                                OperatorID = (int)reader["OperatorID"],
                                OperatorName = reader["OperatorName"].ToString()
                            };
                            
                            userList.Add(op);
                        }
                    }
                }
            }

            return userList;
        }
    }
    public class OperatorResponse
    {
        public int OperatorID { get; set; }
        public string OperatorName { get; set; }
    }
}
