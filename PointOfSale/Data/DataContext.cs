using Microsoft.EntityFrameworkCore;
using PointOfSale.Models;

namespace PointOfSale.Data
{
    public class DataContext : DbContext    
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {  
        }
        public DbSet<Registration> registrations { get; set; }  
    }
}
