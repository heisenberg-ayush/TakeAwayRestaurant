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
        public IEnumerable<ProductTypeResponse> GetProductType()
        {
            var productTypeList = new List<ProductTypeResponse>();

            // Establish a connection to the database
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString());
            conn.Open();

            // Prepare the SQL query to retrieve all registered users
            var query = "SELECT ProductTypeID, ProductTypeName FROM ProductType";

            using (var command = new SqlCommand(query, conn))
            {
                // Execute the query and retrieve the usernames
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Add each username to the list
                        ProductTypeResponse op = new ProductTypeResponse
                        {
                            ProductTypeID = (int)reader["ProductTypeID"],
                            ProductTypeName = reader["ProductTypeName"].ToString()
                        };

                        productTypeList.Add(op);
                    }
                }
            }
            return productTypeList;
        }

        // GET api/<ProductController>/5
        // get all products with the given productTypeId

        [HttpGet("{id}")]
        public IEnumerable<ProductResponse> GetProduct(int id)
        {
            var productList = new List<ProductResponse>();

            // Establish a connection to the database
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString());
            conn.Open();

            // Prepare the SQL query to retrieve all registered users
            var query = "SELECT ProductID, ProductName, ProductTypeID, ProductImage, rate FROM Product WHERE ProductTypeID = @ID";

            using (var command = new SqlCommand(query, conn))
            {
                // Set the parameters for the query
                command.Parameters.AddWithValue("@ID", id);

                // Execute the query and retrieve the products
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Add each product to the list
                        ProductResponse op = new ProductResponse
                        {
                            ProductID = (int)reader["ProductID"],
                            ProductName = reader["ProductName"].ToString(),
                            ProductTypeID = (int)reader["ProductTypeID"],
                            ProductImage = reader["ProductImage"].ToString(),
                            rate = (decimal)reader["rate"]
                        };

                        productList.Add(op);
                    }
                }
            }
            return productList;
        }

        // POST api/<ProductTypeController>
        [HttpPost]
        public string Post(ProductType productType)
        {
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Lazzat").ToString());

            var query = "INSERT INTO ProductType (ProductTypeName, Description, DontShowOnSite, WebSiteDisPlayOrder, ProducTypeImage, OfferTitle, OfferPercent, ProductCategoryID) VALUES (@Name, @Description, @DontShowOnSite, @WebSiteDisPlayOrder, @Image, @OfferTitle, @OfferPercent, @Category)";

            using (var command = new SqlCommand(query, conn))
            {
                // Set the parameters for the query
                command.Parameters.AddWithValue("@Name", productType.ProductTypeName);
                command.Parameters.AddWithValue("@Description", productType.Description);
                command.Parameters.AddWithValue("@DontShowOnSite", productType.DontShowOnSite);
                command.Parameters.AddWithValue("@WebSiteDisPlayOrder", productType.WebSiteDisPlayOrder);
                command.Parameters.AddWithValue("@Image", productType.ProducTypeImage);
                command.Parameters.AddWithValue("@OfferTitle", productType.OfferTitle);
                command.Parameters.AddWithValue("@OfferPercent", productType.OfferPercent);
                command.Parameters.AddWithValue("@Category", productType.ProductCategoryID);

                // Execute the query and retrieve the inserted product ID
                conn.Open();
                int insertedProductId = Convert.ToInt32(command.ExecuteScalar());

                conn.Close();
                return "Data Inserted";
            }
        }

        // PUT api/<ProductTypeController>/5
        [HttpPut("{id}")]
        public ActionResult<ProductTypeResponse> UpdateProductType(int id, ProductType productType)
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
                conn.Open();

                // Prepare the SQL query to update the product
                var query = "UPDATE ProductType SET ProductTypeName = @ProductTypeName, Description = @Description, DontShowOnSite = @DontShowOnSite, WebSiteDisPlayOrder = @WebSiteDisPlayOrder, ProducTypeImage = @ProducTypeImage, OfferTitle = @OfferTitle, OfferPercent = @OfferPercent, ProductCategoryID = @ProductCategoryID WHERE ProductTypeID = @ID";

                using (var command = new SqlCommand(query, conn))
                {
                    // Set the parameters for the query
                    command.Parameters.AddWithValue("@ProductTypeName", productType.ProductTypeName);
                    command.Parameters.AddWithValue("@Description", productType.Description);
                    command.Parameters.AddWithValue("@DontShowOnSite", productType.DontShowOnSite);
                    command.Parameters.AddWithValue("@WebSiteDisPlayOrder", productType.WebSiteDisPlayOrder);
                    command.Parameters.AddWithValue("@ProducTypeImage", productType.ProducTypeImage);
                    command.Parameters.AddWithValue("@OfferTitle", productType.OfferTitle);
                    command.Parameters.AddWithValue("@OfferPercent", productType.OfferPercent);
                    command.Parameters.AddWithValue("@ProductCategoryID", productType.ProductCategoryID);
                    command.Parameters.AddWithValue("@ID", id);

                    // Execute the update query
                    command.ExecuteNonQuery();
                }
            }

            // Return the updated product
            var updatedProduct = new ProductTypeResponse
            {
                ProductTypeID = id,
                ProductTypeName = productType.ProductTypeName
        };

            return Ok(updatedProduct);
        }

        // DELETE api/<ProductTypeController>/5
        [HttpDelete("{id}")]
        public ActionResult DeleteProductType(int id)
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
                conn.Open();

                // Prepare the SQL query to delete the product
                var query = "DELETE FROM ProductType WHERE ProductTypeID = @ID";

                using (var command = new SqlCommand(query, conn))
                {
                    // Set the parameter for the query
                    command.Parameters.AddWithValue("@ID", id);

                    // Execute the delete query
                    command.ExecuteNonQuery();
                }
            }

            return NoContent();
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
