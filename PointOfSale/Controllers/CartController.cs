using Dapper;
using Microsoft.AspNetCore.Mvc;
using PointOfSale.Models;
using System.Data.SqlClient;

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
        [HttpPost]
        public async Task<IActionResult> Post(CartDetail cart)
        {
            try
            {
                using (var conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"INSERT INTO [Lazzatt].[dbo].[CartDetail]
                                       ([DetailSerial]
                                       ,[ProductID]
                                       ,[Quantity]
                                       ,[UnitPrice]
                                       ,[Amount]
                                       ,[Remarks])
                                 VALUES
                                       (@DetailSerial, @ProductID, @Quantity, @UnitPrice, @Amount, @Remarks);
                                 SELECT CAST(SCOPE_IDENTITY() as int)";

                    var newCart = new CartDetail()
                    {
                        DetailSerial = cart.DetailSerial,
                        ProductID = cart.ProductID,
                        Quantity = cart.Quantity,
                        UnitPrice = cart.UnitPrice,
                        Amount = cart.Amount,
                        Remarks = cart.Remarks
                    };

                    var result = await conn.ExecuteAsync(sql, newCart);

                    // Fill the Cart Database that stores the customerID and CartID
                    int newId = await conn.QuerySingleAsync<int>(sql, newCart);

                    return Ok("Data Inserted");
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
        public void Delete(int id)
        {
        }
    }
}
