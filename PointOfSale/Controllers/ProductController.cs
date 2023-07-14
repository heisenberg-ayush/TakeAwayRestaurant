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
            var sql = @"SELECT [ProductID]
                              ,[ProductName]
                              ,[ProductTypeID]
                              ,[Rate]
                              ,[ProductImage] FROM [Lazzatt].[dbo].[Product]";

            using (var conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
            {
                var productList = await conn.QueryAsync<ProductResponse>(sql);
                return Ok(productList);
            }
        }    

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            // Establish a connection to the database
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString());
            var sql = @"SELECT [ProductID]
                              ,[ProductName]
                              ,[ProductTypeID]
                              ,[Rate]
                              ,[ProductImage]
                          FROM [Lazzatt].[dbo].[Product]
                          WHERE ProductID = @ID";

            // Execute the query and retrieve the products using Dapper
            var product = await conn.QueryAsync<ProductResponse>(sql, new {ID = id});

            return Ok(product);
        }

        // POST api/<ProductController>
        [HttpPost]
        public async Task<IActionResult> Post(Product product)
        {
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString());

            var sql = @"INSERT INTO [Lazzatt].[dbo].[Product]
                               ([ProductName]
                              ,[ProductTypeID]
                              ,[Rate]
                              ,[Discontinued]
                              ,[ProductImage]
                              ,[HSN_Code]
                              ,[UOM]
                              ,[PartnerRate]
                              ,[Description]
                              ,[ServeFor]
                              ,[FoodprepareTime]
                              ,[VeganType]
                              ,[WebSiteDisPlayOrder]
                              ,[OfferInCartToBuy]
                              ,[ProductCategoryID]
                              ,[IsOutOfStock])
                            VALUES
                                (@ProductName, @ProductTypeID, @Rate, @Discontinued, @ProductImage, 
                                @HSN_Code, @UOM, @PartnerRate, @Description, @ServeFor, @FoodprepareTime,
                                @VeganType, @WebSiteDisPlayOrder, @OfferInCartToBuy, 
                                @ProductCategoryID, @IsOutOfStock)";

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

            var result = await conn.ExecuteAsync(sql, newProduct);
            return Ok("Data Inserted");
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Product product)
        {
            // Check if the product exists with the given ID
            bool productExists = CheckIfProductTypeExists(id);

            if (!productExists)
            {
                return NotFound("Product not Found");
            }

            SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString());

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
                            WHERE
                              ProductID = @ProductID";

            var newProduct = new Product()
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

            var result = await conn.ExecuteAsync(sql, newProduct);

            return Ok("Updated");
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Check if the product exists with the given ID
            bool productExists = CheckIfProductTypeExists(id);

            if (!productExists)
            {
                return NotFound();
            }

            SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString());

            var sql = @"DELETE FROM [Lazzatt].[dbo].[Product]
                               WHERE ProductID = @ProductID";

            var result = await conn.ExecuteAsync(sql, new { ProductID = id });
            return Ok("Product Deleted");          
        }

        // Helper method to check if the productType exists with the given ID
        private bool CheckIfProductTypeExists(int id)
        {
            // Establish a connection to the database
            using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
            {
                conn.Open();

                // Prepare the SQL query to check if the product exists
                var query = "SELECT COUNT(*) FROM Product WHERE ProductID = @ID";

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
