using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PointOfSale.Models;
using System.Data.SqlClient;
using System.Net;

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
            try
            {
                // Establish a connection to the database
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString()))
                {
                    await conn.OpenAsync();
                    var sql = @"SELECT [ProductCategoryID], [ProductCategoryName]
                        FROM [Lazzatt].[dbo].[ProductCategory]";

                    // Execute the query and retrieve the product categories using Dapper asynchronously
                    var productCategoryList = await conn.QueryAsync<ProductCategory>(sql);

                    return Ok(productCategoryList);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                string errorMessage = "An error occurred while retrieving the product categories";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return StatusCode((int)statusCode, errorMessage);
            }
        }

        // GET api/<ProductCategoryController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetType(int id)
        {
            try
            {
                // Establish a connection to the database
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString()))
                {
                    await conn.OpenAsync();
                    var sql = @"SELECT [ProductTypeID], [ProductTypeName]
                        FROM [Lazzatt].[dbo].[ProductType]
                        WHERE ProductCategoryID = @ID";

                    // Execute the query and retrieve the products using Dapper asynchronously
                    // productTypeList = conn.Query<ProductTypeResponse>(sql, new { ID = id }).ToList();
                    var productTypeList = await conn.QueryAsync<ProductTypeResponse>(sql, new { ID = id });

                    return Ok(productTypeList);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                string errorMessage = "An error occurred while retrieving the product types";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return StatusCode((int)statusCode, errorMessage);
            }
        }

        // POST api/<ProductCategoryController>
        [HttpPost]
        public async Task<IActionResult> PostCategory(ProductCategory category)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"INSERT INTO [Lazzatt].[dbo].[ProductCategory] ([ProductCategoryName])
                        VALUES (@ProductCategoryName)";

                    await conn.ExecuteAsync(sql, new { ProductCategoryName = category.ProductCategoryName });

                    return Ok("Data Inserted");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                string errorMessage = "An error occurred while inserting the product category";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return StatusCode((int)statusCode, errorMessage);
            }
        }

        // PUT api/<ProductCategoryController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, ProductCategory category)
        {
            // Check if the product exists with the given ID
            bool productExists = await CheckIfProductCategoryExistsAsync(id);

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

                try
                {
                    await conn.ExecuteAsync(sql, new { ProductCategoryName = category.ProductCategoryName, ProductCategoryID = id });
                    return Ok("Updated");
                }
                catch (Exception ex)
                {
                    // Handle the exception, log the error, and return an appropriate error response
                    // For example:
                    // logger.LogError(ex, "Error updating customer");
                    return StatusCode(500, "An error occurred while updating the customer");
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            // Check if the product exists with the given ID
            bool productExists = await CheckIfProductCategoryExistsAsync(id);

            if (!productExists)
            {
                return NotFound();
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"DELETE FROM [Lazzatt].[dbo].[ProductCategory]
                        WHERE ProductCategoryID = @ProductCategoryID";

                    await conn.ExecuteAsync(sql, new { ProductCategoryID = id });

                    return Ok("Product Deleted");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                string errorMessage = "An error occurred while deleting the product category";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return StatusCode((int)statusCode, errorMessage);
            }
        }

        // Helper method to check if the productCategory exists with the given ID
        private async Task<bool> CheckIfProductCategoryExistsAsync(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString()))
                {
                    // Prepare the SQL query to check if the product exists
                    var query = "SELECT COUNT(*) FROM ProductCategory WHERE ProductCategoryID = @ID";

                    // Execute the query and retrieve the count asynchronously using Dapper
                    int count = await conn.ExecuteScalarAsync<int>(query, new { ID = id });

                    // If the count is greater than 0, the product exists
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, log the error, and return false (assuming product doesn't exist)
                // For example:
                // logger.LogError(ex, "Error checking if product type exists");
                return false;
            }
        }

    }
}
