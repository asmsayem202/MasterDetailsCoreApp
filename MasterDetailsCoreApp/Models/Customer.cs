using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterDetailsCoreApp.Models
{
    public class Customer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public bool IsPermanent { get; set; }
        public string? Image { get; set; }


        [NotMapped]
        public IFormFile? ImageUpload { get; set; }
        public IList<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    }
}
