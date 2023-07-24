using Dapper;
using Microsoft.AspNetCore.Mvc;
using PointOfSale.Models;
using System.Data.SqlClient;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PointOfSale.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerOrderController : ControllerBase
    {
        private readonly IConfiguration _config;
        public CustomerOrderController(IConfiguration config)
        {
            _config = config;
        }

        // GET api/<CustomerOrderController>/5
        // takes customer ID
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                using (var conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"SELECT [CustomerOrderID]
                          ,[CustomerID], [OrderNumber], [OrderDate], [OrderType], [OnlinePartnerID], [PartnerOrderNumber], [AddressID], [DeliveryDate], [DeliveryTime]
                          ,[DeliveryLocation], [TotalValue], [OrderInstance], [IsDeliveryApplicable], [KitchenOrderTicketID], [SaleBillID], [IsCancel], [IsNoSpoon]
                          ,[IsCustomerApplication], [PointsRedeemed], [PaymentMethod], [PointsEarned], [PointRedeemAmount], [SGST_Rate], [CGST_Rate], [SGST_Amount]
                          ,[CGST_Amount], [OrderStatus], [Remarks], [NetPayable], [RoundOff], [CustomerName], [MobileNumber], [IsReturn]
                      FROM [Lazzatt].[dbo].[CustomerOrder]
                      WHERE CustomerID = @ID";

                    var CustomerOrderID = await conn.QueryFirstOrDefaultAsync<int>(sql, new { ID = id });

                    if (CustomerOrderID == null)
                    {
                        return NotFound(); // Return 404 if CustomerOrder is not found
                    }

                    return Ok(CustomerOrderID);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately, log the error, or return a specific error response
                return StatusCode(500, "An error occurred while retrieving cart data.");
            }
        }

        // POST api/<CustomerOrderController>
        [HttpPost]
        public async Task<IActionResult> Post(int id, CustomerOrder order)
        {
            try
            {
                using (var conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    // Get customer detail
                    var sql = @"SELECT [CustomerID]
                            ,[CustomerName]
                            ,[CustomerMobile]
                            ,[Email]
                        FROM [Lazzatt].[dbo].[Customer]
                        WHERE CustomerID = @ID";

                    var customer = await conn.QueryFirstOrDefaultAsync<CustomerResponse>(sql, new { ID = id });

                    if (customer == null)
                    {
                        return NotFound(); // Return 404 if customer is not found
                    }

                    // Get Cart 
                    var getCartSql = @"SELECT [CartID]
                          FROM [Lazzatt].[dbo].[Cart]
                          WHERE CustomerID = @ID
                          ORDER BY [CartID] DESC";

                    var cartID = await conn.QueryFirstOrDefaultAsync<int>(getCartSql, new { ID = id });

                    if (cartID == null)
                    {
                        return NotFound("No Cart Found"); // Return 404 if cart is not found
                    }

                    // Calculate total payable amount
                    var getCartDetailSql = @"SELECT SUM([Amount]) AS TotalAmount
                                            FROM [Lazzatt].[dbo].[CartDetail]
                                            WHERE CartID = @ID";

                    var totalAmt = await conn.QueryFirstOrDefaultAsync<decimal>(getCartDetailSql, new { ID = cartID });

                    if (totalAmt == null)
                    {
                        return NotFound("No products added in Cart"); // Return 404 if product is not found
                    }

                    var insertSql = @"INSERT INTO [Lazzatt].[dbo].[CustomerOrder]
                           ([CustomerID],[OrderNumber],[OrderDate],[OnlinePartnerID],[PartnerOrderNumber],[OrderType],[AddressID]
                           ,[DeliveryDate],[DeliveryTime],[DeliveryLocation],[TotalValue],[OrderInstance],[IsDeliveryApplicable]
                           ,[IsNoSpoon],[PaymentMethod],[SGST_Rate],[CGST_Rate],[SGST_Amount],[CGST_Amount]
                           ,[Remarks],[NetPayable],[RoundOff],[CustomerName],[MobileNumber],[IsReturn])
                     VALUES
                           (@CustomerID, @OrderNumber, @OrderDate, @OnlinePartnerID, @PartnerOrderNumber, @OrderType, @AddressID, @DeliveryDate, @DeliveryTime
                            ,@DeliveryLocation, @TotalValue, @OrderInstance, @IsDeliveryApplicable, @IsNoSpoon
                            ,@PaymentMethod, @SGST_Rate, @CGST_Rate, @SGST_Amount
                            ,@CGST_Amount, @Remarks, @NetPayable, @RoundOff, @CustomerName, @MobileNumber, @IsReturn);
                     SELECT CAST(SCOPE_IDENTITY() as int);";

                    const double V = 2.5;
                    var newOrder = new CustomerOrder()
                    {
                        CustomerID = id,
                        OrderNumber = order.OrderNumber,
                        OrderDate = order.OrderDate,
                        OnlinePartnerID = order.OnlinePartnerID,
                        PartnerOrderNumber = order.PartnerOrderNumber,
                        OrderType = order.OrderType,
                        AddressID = order.AddressID,
                        DeliveryDate = order.DeliveryDate,
                        DeliveryTime = order.DeliveryTime,
                        DeliveryLocation = order.DeliveryLocation,
                        TotalValue = totalAmt,
                        OrderInstance = order.OrderInstance,
                        IsDeliveryApplicable = order.IsDeliveryApplicable,
                        IsNoSpoon = order.IsNoSpoon,
                        PaymentMethod = order.PaymentMethod,
                        SGST_Rate = (decimal)V,
                        CGST_Rate = (decimal)V,
                        SGST_Amount = 0.025M * totalAmt,
                        CGST_Amount = 0.025M * totalAmt,
                        Remarks = order.Remarks,
                        NetPayable = Math.Round(totalAmt + (0.025M * totalAmt) + (0.025M * totalAmt), 2),
                        RoundOff = order.RoundOff,
                        CustomerName = customer.CustomerName,
                        MobileNumber = customer.CustomerMobile,
                        IsReturn = order.IsReturn,
                    };

                    await conn.ExecuteAsync(insertSql, newOrder);

                    // Get CustomerOrderID
                   /* int newId = await conn.QuerySingleAsync<int>(sql, newOrder);
                    var orderResponse = new CustomerOrderResponse()
                    {
                        CustomerCartID = cartID,
                        CustomerOrderID = newId,
                    };*/

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately, log the error, or return a specific error response
                return StatusCode(500, "An error occurred while inserting customer order data.");
            }
        }

        /*// PUT api/<CustomerOrderController>/5
        //takes Customer ID
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, CustomerOrder order)
        {
            // Check if the customer order exists with the given ID
            bool orderExists = await CheckIfCustomerOrderExistsAsync(id);

            if (!orderExists)
            {
                return NotFound("Customer order not Found");
            }

            SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString());

            var sql = @"UPDATE [Lazzatt].[dbo].[CustomerOrder]
                               SET OrderNumber = @OrderNumber, 
                                   OrderDate = @OrderDate, 
                                   OrderType = @OrderType, 
                                   OnlinePartnerID = @OnlinePartnerID, 
                                   PartnerOrderNumber = @PartnerOrderNumber, 
                                   AddressID = @AddressID, 
                                   DeliveryDate = @DeliveryDate, 
                                   DeliveryTime = @DeliveryTime,
                                   DeliveryLocation = @DeliveryLocation, 
                                   TotalValue = @TotalValue, 
                                   OrderInstance = @OrderInstance, 
                                   IsDeliveryApplicable = @IsDeliveryApplicable, 
                                   KitchenOrderTicketID = @KitchenOrderTicketID, 
                                   SaleBillID = @SaleBillID, 
                                   IsCancel = @IsCancel, 
                                   IsNoSpoon = @IsNoSpoon,
                                   IsCustomerApplication = @IsCustomerApplication, 
                                   PointsRedeemed = @PointsRedeemed, 
                                   PaymentMethod = @PaymentMethod, 
                                   PointsEarned = @PointsEarned, 
                                   PointRedeemAmount = @PointRedeemAmount,
                                   OrderStatus = @OrderStatus, 
                                   Remarks = @Remarks, 
                                   @IsReturn
                            WHERE
                                CustomerID = @CustomerID;";

            try
            {
                await conn.ExecuteAsync(sql, order);
                return Ok("Updated");
            }
            catch (Exception ex)
            {
                // Handle the exception, log the error, and return an appropriate error response
                // For example:
                // logger.LogError(ex, "Error updating customer order");
                return StatusCode(500, "An error occurred while updating the customer order");
            }
        }*/

        // DELETE api/<CustomerOrderController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Check if the customer order exists with the given ID
                bool orderExists = await CheckIfCustomerOrderExistsAsync(id);

                if (!orderExists)
                {
                    return NotFound("Customer Order not Found");
                }

                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    var sql = @"DELETE FROM [Lazzatt].[dbo].[CustomerOrder]
                        WHERE CustomerID = @CustomerID";

                    await conn.ExecuteAsync(sql, new { OrderID = id });

                    return Ok("Customer Order Deleted");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error response
                string errorMessage = "An error occurred while deleting the customer order";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                // Additional error handling logic if needed
                // For example, you can log the error or provide more specific error messages based on the exception type

                return StatusCode((int)statusCode, errorMessage);
            }
        }

        private async Task<bool> CheckIfCustomerOrderExistsAsync(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("Lazzat").ToString()))
                {
                    // Prepare the SQL query to check if the customer order exists
                    var query = "SELECT COUNT(*) FROM CustomerOrder WHERE CustomerID = @ID";

                    // Execute the query and retrieve the count asynchronously using Dapper
                    int count = await conn.ExecuteScalarAsync<int>(query, new { ID = id });

                    // If the count is greater than 0, the customer order exists
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, log the error, and return false (assuming customer order doesn't exist)
                // For example:
                // logger.LogError(ex, "Error checking if customer order exists");
                return false;
            }
        }
    }

    public class CustomerOrderResponse
    {
        public int CustomerCartID { get; set;}
        public int CustomerOrderID { get; set;}
    }
}
