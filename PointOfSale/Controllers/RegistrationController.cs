using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PointOfSale.Models;
using System.Data.SqlClient;

namespace PointOfSale.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public RegistrationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("registration")]
        public async Task<string> Registration(Registration registration)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("PointOfSale").ToString()))
                {
                    string sql = "INSERT INTO registrations (OperatorName, Password) VALUES (@OperatorName, @Password)";
                    var parameters = new { OperatorName = registration.OperatorName, Password = registration.Password };

                    await conn.ExecuteAsync(sql, parameters);

                    return "Data Inserted";
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, log the error, or provide a specific error message
                // based on your application's requirements
                return "Error";
            }
        }

    }
}
