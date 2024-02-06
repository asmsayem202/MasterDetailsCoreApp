using Microsoft.EntityFrameworkCore;

namespace MasterDetailsCoreApp.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions opt) : base(opt) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Items> Items { get; set; }
        //public DbSet<Items> Items { get; set; }


    }
}
