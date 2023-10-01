using System.ComponentModel.DataAnnotations;

namespace ClientManagement.Models
{
    public partial class Client
    {
        [Key]
        public long ClientId { get; set; }
        public Guid LicenceKey { get; set; }
        public string ClientName { get; set; } = null!;
        public DateTime LicenceStartDate { get; set; }
        public DateTime LicenceEndDate { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
