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
        public IEnumerable<ProductResponse> GetProduct()
        {
            var productList = new List<ProductResponse>();

            // Establish a connection to the database
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString());
            conn.Open();

            // Prepare the SQL query to retrieve all registered users
            var query = "SELECT ProductID, ProductName, ProductTypeID, Rate, ProductImage FROM Product";

            using (var command = new SqlCommand(query, conn))
            {
                // Execute the query and retrieve the usernames
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
                            rate = (decimal)reader["Rate"]
                        };

                        productList.Add(op);
                    }
                }
            }
            return productList;
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ProductController>
        [HttpPost]
        public string Post(Product product)
        {
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString());

            var query = "INSERT INTO Product (ProductName, ProductTypeID, Rate, " +
                "Discontinued, ProductImage, HSN_Code, UOM, PartnerRate, Description, " +
                "ServeFor, FoodprepareTime, CuisineType, VeganType, WebSiteDisPlayOrder, " +
                "OfferInCartToBuy, ProductCategoryID, IsOutOfStock) VALUES (@Name, @TypeId, @Rate, " +
                "@Discontinued, @Image, @HSN_Code, @UOM, @PartnerRate, @Description, @ServeFor, @Time, " +
                "@CuisineType, @VeganType, @WebSiteDisPlayOrder, @OfferInCartToBuy, @Category, @IsOutOfStock)";

            using (var command = new SqlCommand(query, conn))
            {
                // Set the parameters for the query
                command.Parameters.AddWithValue("@Name", product.ProductName);
                command.Parameters.AddWithValue("@TypeId", product.Description);
                command.Parameters.AddWithValue("@Rate", product.Rate);
                command.Parameters.AddWithValue("@Discontinued", product.Discontinued);
                command.Parameters.AddWithValue("@Image", product.ProductImage);
                command.Parameters.AddWithValue("@HSN_Code", product.HSN_Code);
                command.Parameters.AddWithValue("@UOM", product.UOM);
                command.Parameters.AddWithValue("@PartnerRate", product.PartnerRate);
                command.Parameters.AddWithValue("@Description", product.Description);
                command.Parameters.AddWithValue("@ServeFor", product.ServeFor);
                command.Parameters.AddWithValue("@Time", product.PartnerRate);
                command.Parameters.AddWithValue("@CuisineType", product.FoodprepareTime);
                command.Parameters.AddWithValue("@VeganType", product.VeganType);
                // command.Parameters.AddWithValue("@IsVeg", product.IsVeg);
                command.Parameters.AddWithValue("@WebSiteDisPlayOrder", product.WebSiteDisPlayOrder);
                command.Parameters.AddWithValue("@OfferInCartToBuy", product.OfferInCartToBuy);
                command.Parameters.AddWithValue("@Category", product.ProductCategoryID);
                command.Parameters.AddWithValue("@IsOutOfStock", product.IsOutOfStock);

                // Execute the query and retrieve the inserted product ID
                conn.Open();
                // Execute the query
                command.ExecuteNonQuery();
                conn.Close();

                return "Data Inserted";
            }
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
