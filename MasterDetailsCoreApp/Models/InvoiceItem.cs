using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MasterDetailsCoreApp.Models
{
    public class InvoiceItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime InvoiceDate { get; set; }
        public int Quantity { get; set; }
        public decimal ItemTotal => this.Items is null ? 0 : this.Items.Price * this.Quantity;


        public int? ItemsId { get; set; }
        public int? CustomerId { get; set; }
        public Items? Items { get; set; }
        public Customer? Customer { get; set; }
    }
}
