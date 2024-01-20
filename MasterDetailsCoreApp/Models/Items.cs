using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MasterDetailsCoreApp.Models
{
    public class Items
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public decimal Price { get; set; }


        public IList<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    }
}
