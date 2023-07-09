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
        public string registration(Registration registration)

        {
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("PointOfSale").ToString());
            SqlCommand cmd = new SqlCommand("INSERT INTO registrations(OperatorName, Password) VALUES ('" + registration.OperatorName + "', '" + registration.Password + "')", conn);
            conn.Open();
            int i = cmd.ExecuteNonQuery();
            conn.Close();
            if (i > 0)
                return "Data Inserted";
            else
                return "Error";
        }
    }
}
