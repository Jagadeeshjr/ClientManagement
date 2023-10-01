using ClientManagement.Models;

namespace ClientManagement
{
    public class PagedClientResult
    {
        public List<Client> Clients { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }
    }
}
