using Dapper;
using Microsoft.AspNetCore.Mvc;
using PointOfSale.Models;
using System.Data.SqlClient;
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
            // Establish a connection to the database
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString());

            var sql = @"SELECT [ProductTypeID]
                              ,[ProductTypeName]
                          FROM [Lazzatt].[dbo].[ProductType]";

            var productTypeList = await conn.QueryAsync<ProductTypeResponse>(sql);
            return Ok(productTypeList);
        }

        // GET api/<ProductController>/5
        // get all products with the given productTypeId

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var productList = new List<ProductResponse>();

            // Establish a connection to the database
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString());
            var sql = @"SELECT [ProductID]
                              ,[ProductName]
                              ,[ProductTypeID]
                              ,[Rate]
                              ,[ProductImage]
                          FROM [Lazzatt].[dbo].[Product]
                          WHERE ProductTypeID = @ID";
            // Execute the query and retrieve the products using Dapper
            productList = conn.Query<ProductResponse>(sql, new { ID = id }).ToList();

            return Ok(productList);
        }

        // POST api/<ProductTypeController>
        [HttpPost]
        public async Task<IActionResult> PostType(ProductType productType)
        {
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString());

            var sql = @"INSERT INTO [Lazzatt].[dbo].[ProductType]
                                ([ProductTypeName]
                              ,[Description]
                              ,[DontShowOnSite]
                              ,[WebSiteDisPlayOrder]
                              ,[ProducTypeImage]
                              ,[OfferTitle]
                              ,[OfferPercent]
                              ,[ProductCategoryID])
                        VALUES (@ProductTypeName, @Description, @DontShowOnSite, @WebSiteDisPlayOrder, @ProducTypeImage, @OfferTitle, @OfferPercent, @ProductCategoryID)";

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

            var result = await conn.ExecuteAsync(sql, newProductType);
            return Ok("Data Inserted");
        }

        // PUT api/<ProductTypeController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductType(int id, ProductType productType)
        {
            // Check if the product exists with the given ID
            bool productExists = CheckIfProductTypeExists(id);

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

                var newProductType = new ProductType()
                {
                    ProductTypeName = productType.ProductTypeName,
                    Description = productType.Description,
                    DontShowOnSite = productType.DontShowOnSite,
                    WebSiteDisPlayOrder = productType.WebSiteDisPlayOrder,
                    ProducTypeImage = productType.ProducTypeImage,
                    OfferTitle = productType.OfferTitle,
                    OfferPercent = productType.OfferPercent,
                    ProductCategoryID = productType.ProductCategoryID,
                    ProductTypeID = id
                };

                var result = await conn.ExecuteAsync(sql, newProductType);
            }
            return Ok("Data Updated");
        }

        // DELETE api/<ProductTypeController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductType(int id)
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
                var sql = @"DELETE FROM [Lazzatt].[dbo].[Product]
                               WHERE ProductTypeID = @ProductTypeID";

                var result = await conn.ExecuteAsync(sql, new { ProductTypeID = id });
            }
            return Ok("Product Deleted");
        }

        // Helper method to check if the productType exists with the given ID
        private bool CheckIfProductTypeExists(int id)
        {
            // Establish a connection to the database
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString()))
            {
                conn.Open();

                // Prepare the SQL query to check if the product exists
                var query = "SELECT COUNT(*) FROM ProductType WHERE ProductTypeID = @ID";

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
