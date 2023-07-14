using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PointOfSale.Models;
using System.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PointOfSale.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ProductCategoryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/<ProductCategoryController>
        [HttpGet]
        public async Task<IActionResult> GetAllTypes()
        {
            // Establish a connection to the database
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString());

            var sql = @"SELECT [ProductCategoryID]
                              ,[ProductCategoryName]
                          FROM [Lazzatt].[dbo].[ProductCategory]";

            var productCategoryList = await conn.QueryAsync<ProductCategory>(sql);
            return Ok(productCategoryList);
        }

        // GET api/<ProductCategoryController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetType(int id)
        {
            var productTypeList = new List<ProductTypeResponse>();

            // Establish a connection to the database
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString());
            var sql = @"SELECT [ProductTypeID]
                              ,[ProductTypeName]
                          FROM [Lazzatt].[dbo].[ProductType]
                          WHERE ProductCategoryID = @ID";
            // Execute the query and retrieve the products using Dapper
            productTypeList = conn.Query<ProductTypeResponse>(sql, new { ID = id }).ToList();

            return Ok(productTypeList);
        }

        // POST api/<ProductCategoryController>
        [HttpPost]
        public async Task<IActionResult> PostCategory(ProductCategory category)
        {
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString());

            var sql = @"INSERT INTO [Lazzatt].[dbo].[ProductCategory]
                                ([ProductCategoryName])
                        VALUES (@ProductCategoryName)";

            var result = await conn.ExecuteAsync(sql, new { ProductCategoryName = category.ProductCategoryName });
            return Ok("Data Inserted");
        }

        // PUT api/<ProductCategoryController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, ProductCategory category)
        {
            // Check if the product exists with the given ID
            bool productExists = CheckIfProductTypeExists(id);

            if (!productExists)
            {
                return NotFound("Category Not Found");
            }

            // Establish a connection to the database
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString()))
            {
                var sql = @"UPDATE [Lazzatt].[dbo].[ProductCategory] 
                                     SET ProductCategoryName = @ProductCategoryName
                                     WHERE ProductCategoryID = @ProductCategoryID";

                var result = await conn.ExecuteAsync(sql, new { ProductCategoryName = category.ProductCategoryName, ProductCategoryID = id });
            }
            return Ok("Data Updated");
        }

        // DELETE api/<ProductCategoryController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            // Check if the product exists with the given ID
            bool productExists = CheckIfProductTypeExists(id);

            if (!productExists)
            {
                return NotFound();
            }

            // Establish a connection to the database
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString()))
            {
                var sql = @"DELETE FROM [Lazzatt].[dbo].[ProductCategory]
                               WHERE ProductCategoryID = @ProductCategoryID";

                var result = await conn.ExecuteAsync(sql, new { ProductCategoryID = id });
            }
            return Ok("Product Deleted");
        }

        // Helper method to check if the productCategory exists with the given ID
        private bool CheckIfProductTypeExists(int id)
        {
            // Establish a connection to the database
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString()))
            {
                conn.Open();

                // Prepare the SQL query to check if the product exists
                var query = "SELECT COUNT(*) FROM ProductCategory WHERE ProductCategoryID = @ID";

                using (var command = new SqlCommand(query, conn))
                {
                    // Set the parameter for the query
                    command.Parameters.AddWithValue("@ID", id);

                    // Execute the query to get the count of matching products
                    int count = (int)command.ExecuteScalar();

                    // If the count is greater than 0, the product exists
                    return count > 0;
                }
            }
        }
    }
}
