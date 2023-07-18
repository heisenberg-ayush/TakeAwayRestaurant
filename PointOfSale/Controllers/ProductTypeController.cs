using Dapper;
using Microsoft.AspNetCore.Mvc;
using PointOfSale.Models;
using System.Data.SqlClient;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PointOfSale.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ProductTypeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/<ProductTypeController>
        [HttpGet]
        public async Task<IActionResult> GetProductType()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"SELECT [ProductTypeID], [ProductTypeName]
                        FROM [Lazzatt].[dbo].[ProductType]";

                    var productTypeList = await conn.QueryAsync<ProductTypeResponse>(sql);

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


        // GET api/<ProductController>/5
        // get all products with the given productTypeId

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"SELECT [ProductID], [ProductName], [ProductTypeID], [Rate], [ProductImage]
                        FROM [Lazzatt].[dbo].[Product]
                        WHERE ProductTypeID = @ID";

                    //  productList = conn.Query<ProductResponse>(sql, new { ID = id }).ToList();
                    var productList = await conn.QueryAsync<ProductResponse>(sql, new { ID = id });

                    return Ok(productList);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                string errorMessage = "An error occurred while retrieving the products";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return StatusCode((int)statusCode, errorMessage);
            }
        }


        // POST api/<ProductTypeController>
        [HttpPost]
        public async Task<IActionResult> PostType(ProductType productType)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"INSERT INTO [Lazzatt].[dbo].[ProductType]
                        ([ProductTypeName], [Description], [DontShowOnSite], [WebSiteDisPlayOrder],
                         [ProducTypeImage], [OfferTitle], [OfferPercent], [ProductCategoryID])
                        VALUES (@ProductTypeName, @Description, @DontShowOnSite, @WebSiteDisPlayOrder,
                                @ProducTypeImage, @OfferTitle, @OfferPercent, @ProductCategoryID)";

                    var newProductType = new ProductType()
                    {
                        ProductTypeName = productType.ProductTypeName,
                        Description = productType.Description,
                        DontShowOnSite = productType.DontShowOnSite,
                        WebSiteDisPlayOrder = productType.WebSiteDisPlayOrder,
                        ProducTypeImage = productType.ProducTypeImage,
                        OfferTitle = productType.OfferTitle,
                        OfferPercent = productType.OfferPercent,
                        ProductCategoryID = productType.ProductCategoryID
                    };

                    await conn.ExecuteAsync(sql, newProductType);

                    return Ok("Data Inserted");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                string errorMessage = "An error occurred while inserting the product type";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return StatusCode((int)statusCode, errorMessage);
            }
        }

        // PUT api/<ProductTypeController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductType(int id, ProductType productType)
        {
            try
            {
                // Check if the product exists with the given ID
                bool productExists = await CheckIfProductTypeExistsAsync(id);

                if (!productExists)
                {
                    return NotFound("Type not Found");
                }

                // Establish a connection to the database
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"UPDATE [Lazzatt].[dbo].[ProductType]
                        SET ProductTypeName = @ProductTypeName,
                            Description = @Description,
                            DontShowOnSite = @DontShowOnSite,
                            WebSiteDisPlayOrder = @WebSiteDisPlayOrder,
                            ProducTypeImage = @ProducTypeImage,
                            OfferTitle = @OfferTitle,
                            OfferPercent = @OfferPercent,
                            ProductCategoryID = @ProductCategoryID
                        WHERE ProductTypeID = @ProductTypeID";

                    var updatedProductType = new ProductType()
                    {
                        ProductTypeID = id,
                        ProductTypeName = productType.ProductTypeName,
                        Description = productType.Description,
                        DontShowOnSite = productType.DontShowOnSite,
                        WebSiteDisPlayOrder = productType.WebSiteDisPlayOrder,
                        ProducTypeImage = productType.ProducTypeImage,
                        OfferTitle = productType.OfferTitle,
                        OfferPercent = productType.OfferPercent,
                        ProductCategoryID = productType.ProductCategoryID
                    };

                    await conn.ExecuteAsync(sql, updatedProductType);
                }

                return Ok("Data Updated");
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                string errorMessage = "An error occurred while updating the product type";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return StatusCode((int)statusCode, errorMessage);
            }
        }


        // DELETE api/<ProductTypeController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductType(int id)
        {
            try
            {
                // Check if the product exists with the given ID
                bool productExists = await CheckIfProductTypeExistsAsync(id);

                if (!productExists)
                {
                    return NotFound();
                }

                // Establish a connection to the database
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"DELETE FROM [Lazzatt].[dbo].[ProductType]
                        WHERE ProductTypeID = @ProductTypeID";

                    await conn.ExecuteAsync(sql, new { ProductTypeID = id });

                    return Ok("Product Type Deleted");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                string errorMessage = "An error occurred while deleting the product";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return StatusCode((int)statusCode, errorMessage);
            }
        }

        // Helper method to check if the productType exists with the given ID
        private async Task<bool> CheckIfProductTypeExistsAsync(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString()))
                {
                    // Prepare the SQL query to check if the product exists
                    var query = "SELECT COUNT(*) FROM ProductType WHERE ProductTypeID = @ID";

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
    public class ProductTypeResponse
    {
        public int ProductTypeID { get; set; }
        public string ProductTypeName { get; set; }
    }
    public class ProductResponse
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int ProductTypeID { get; set; }
        public decimal rate { get; set; }
        public string ProductImage { get; set; }
    }
}
