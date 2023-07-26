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
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _config;
        public ProductController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var sql = @"SELECT [ProductID], [ProductName], [ProductTypeID], [Rate], [ProductImage]
                    FROM [Lazzatt].[dbo].[Product]";

                using (var conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    await conn.OpenAsync();
                    var productList = await conn.QueryAsync<ProductResponse>(sql);
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


        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    await conn.OpenAsync();
                    var sql = @"SELECT [ProductID], [ProductName], [ProductTypeID], [Rate], [ProductImage]
                        FROM [Lazzatt].[dbo].[Product]
                        WHERE ProductID = @ID";

                    var product = await conn.QuerySingleOrDefaultAsync<ProductResponse>(sql, new { ID = id });

                    if (product == null)
                    {
                        return NotFound();
                    }

                    return Ok(product);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                string errorMessage = "An error occurred while retrieving the product";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return StatusCode((int)statusCode, errorMessage);
            }
        }


        // POST api/<ProductController>
        [HttpPost]
        public async Task<IActionResult> Post(Product product)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"INSERT INTO [Lazzatt].[dbo].[Product]
                        ([ProductName], [ProductTypeID], [Rate], [Discontinued], [ProductImage],
                         [HSN_Code], [UOM], [PartnerRate], [Description], [ServeFor], [FoodprepareTime],
                         [VeganType], [WebSiteDisPlayOrder], [OfferInCartToBuy], [ProductCategoryID],
                         [IsOutOfStock])
                        VALUES
                        (@ProductName, @ProductTypeID, @Rate, @Discontinued, @ProductImage,
                         @HSN_Code, @UOM, @PartnerRate, @Description, @ServeFor, @FoodprepareTime,
                         @VeganType, @WebSiteDisPlayOrder, @OfferInCartToBuy, @ProductCategoryID,
                         @IsOutOfStock)";

                    var newProduct = new Product()
                    {
                        ProductName = product.ProductName,
                        ProductTypeID = product.ProductTypeID,
                        Rate = product.Rate,
                        Discontinued = product.Discontinued,
                        ProductImage = product.ProductImage,
                        HSN_Code = product.HSN_Code,
                        UOM = product.UOM,
                        PartnerRate = product.PartnerRate,
                        Description = product.Description,
                        ServeFor = product.ServeFor,
                        FoodprepareTime = product.FoodprepareTime,
                        VeganType = product.VeganType,
                        WebSiteDisPlayOrder = product.WebSiteDisPlayOrder,
                        OfferInCartToBuy = product.OfferInCartToBuy,
                        ProductCategoryID = product.ProductCategoryID,
                        IsOutOfStock = product.IsOutOfStock
                    };

                    await conn.ExecuteAsync(sql, newProduct);

                    return Ok("Data Inserted");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                string errorMessage = "An error occurred while inserting the product";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return StatusCode((int)statusCode, errorMessage);
            }
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Product product)
        {
            try
            {
                // Check if the product exists with the given ID
                bool productExists = await CheckIfProductExistsAsync(id);

                if (!productExists)
                {
                    return NotFound("Product not Found");
                }

                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"UPDATE [Lazzatt].[dbo].[Product]
                        SET ProductName = @ProductName,
                            ProductTypeID = @ProductTypeID,
                            Rate = @Rate,
                            Discontinued = @Discontinued,
                            ProductImage = @ProductImage,
                            HSN_Code = @HSN_Code,
                            UOM = @UOM,
                            PartnerRate = @PartnerRate,
                            Description = @Description,
                            ServeFor = @ServeFor,
                            FoodprepareTime = @FoodprepareTime,
                            VeganType = @VeganType,
                            WebSiteDisPlayOrder = @WebSiteDisPlayOrder,
                            OfferInCartToBuy = @OfferInCartToBuy,
                            ProductCategoryID = @ProductCategoryID,
                            IsOutOfStock = @IsOutOfStock
                        WHERE ProductID = @ProductID";

                    var updatedProduct = new Product()
                    {
                        ProductID = id,
                        ProductName = product.ProductName,
                        ProductTypeID = product.ProductTypeID,
                        Rate = product.Rate,
                        Discontinued = product.Discontinued,
                        ProductImage = product.ProductImage,
                        HSN_Code = product.HSN_Code,
                        UOM = product.UOM,
                        PartnerRate = product.PartnerRate,
                        Description = product.Description,
                        ServeFor = product.ServeFor,
                        FoodprepareTime = product.FoodprepareTime,
                        VeganType = product.VeganType,
                        WebSiteDisPlayOrder = product.WebSiteDisPlayOrder,
                        OfferInCartToBuy = product.OfferInCartToBuy,
                        ProductCategoryID = product.ProductCategoryID,
                        IsOutOfStock = product.IsOutOfStock
                    };

                    await conn.ExecuteAsync(sql, updatedProduct);
                }

                return Ok("Updated");
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                string errorMessage = "An error occurred while updating the product";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return StatusCode((int)statusCode, errorMessage);
            }
        }


        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Check if the product exists with the given ID
                bool productExists = await CheckIfProductExistsAsync(id);

                if (!productExists)
                {
                    return NotFound();
                }

                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"DELETE FROM [Lazzatt].[dbo].[Product]
                        WHERE ProductID = @ProductID";

                    await conn.ExecuteAsync(sql, new { ProductID = id });

                    return Ok("Product Deleted");
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
        private async Task<bool> CheckIfProductExistsAsync(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    // Prepare the SQL query to check if the product exists
                    var query = "SELECT COUNT(*) FROM Product WHERE ProductID = @ID";

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
