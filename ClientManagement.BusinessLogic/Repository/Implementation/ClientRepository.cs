using ClientManagement.Caching;
using ClientManagement.Models;
using LazyCache;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using ClientManagement.BusinessLogic.Repository.Contracts;

namespace ClientManagement.BusinessLogic.Repository.Implementation
{
    public class ClientRepository : IClientRepository
    {
        private readonly ClientManagementContext _context;
        private ICacheProvider _cacheProvider;

        public ClientRepository(ClientManagementContext context, ICacheProvider cacheProvider)
        {
            _context = context;
            _cacheProvider = cacheProvider;
        }

        public async Task<PagedClientResult> GetAllclientsBySortingAsync(string term, string? sort, int page, int limit)
        {
            IQueryable<Client> clients;
            if (string.IsNullOrWhiteSpace(term))
            {
                clients = _context.Clients;
            }
            else
            {
                term = term.Trim().ToLower();

                clients = _context
                    .Clients
                    .Where(x => x.ClientName.ToLower().Contains(term)
                    || x.Description.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(sort))
            {
                var sortFields = sort.Split(',');
                StringBuilder orderQueryBuilder = new StringBuilder();

                PropertyInfo[] propertyInfo = typeof(Client).GetProperties();

                foreach (var field in sortFields)
                {
                    string sortOrder = "ascending";
                    var sortField = field.Trim();
                    if (sortField.StartsWith('-'))
                    {
                        sortField = sortField.TrimStart('-');
                        sortOrder = "descending";
                    }
                    var property = propertyInfo.FirstOrDefault(a =>
                                    a.Name.Equals(sortField,
                                    StringComparison.OrdinalIgnoreCase));
                    if (property == null)
                    {
                        continue;
                    }
                    orderQueryBuilder.Append($"{property.Name.ToString()}" +
                                             $"{sortOrder}, ");
                }

                string orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
                if (!string.IsNullOrWhiteSpace(orderQuery))
                {
                    clients = clients.OrderBy(orderQuery);
                }
                else
                {
                    clients = clients.OrderBy(x => x.ClientId);
                }
            }

            //applying pagination
            var totalCount = await _context.Clients.CountAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)limit);

            var pagedClients = await clients.Skip((page - 1) * limit).Take(limit).ToListAsync();

            var pagedEmployeeData = new PagedClientResult
            {
                Clients = pagedClients,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
            return pagedEmployeeData;
        }


        public async Task<List<Client>> GetAllClientsAsync()
        {
            if (!_cacheProvider.TryGetValue(CacheKey.Client, out List<Client> clients))
            {
                clients = await _context.Clients.ToListAsync();

                var cacheEntryOption = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(30),
                    SlidingExpiration = TimeSpan.FromSeconds(30),
                    Size = 1024
                };
                _cacheProvider.Set(CacheKey.Client, clients, cacheEntryOption);
            }
            return clients;
        }

        public async Task<Client> GetClientByIdAsync(long id)
        {
            if (!_cacheProvider.TryGetValue(CacheKey.Client, out Client client))
            {
                client = await _context.Clients.FindAsync(id);

                var cacheEntryOption = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(30),
                    SlidingExpiration = TimeSpan.FromSeconds(30),
                    Size = 1024
                };
                _cacheProvider.Set(CacheKey.Client, client, cacheEntryOption);
            }
            return client;
        }

        public async Task<bool> PatientExistsAsync(long id)
        {
            return await _context.Clients.AnyAsync(x => x.ClientId == id);
        }

        public async Task<long> AddClientAsync(Client clientModel)
        {
            //clientModel.LicenceKey = new Guid();
            clientModel.LicenceKey = Guid.NewGuid();

            clientModel.LicenceStartDate = DateTime.Now;
            clientModel.LicenceEndDate = DateTime.Now.AddYears(3);

            clientModel.CreatedDate = DateTime.Now;
            _context.Clients.Add(clientModel);
            await _context.SaveChangesAsync();
            return clientModel.ClientId;
        }

        public async Task UpdateClientAsync(Client clientModel)
        {
            clientModel.UpdatedDate = DateTime.Now;
            _context.Clients.Update(clientModel);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateClientPatchAsync(long id, JsonPatchDocument clientModel)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client != null)
            {
                client.UpdatedDate = DateTime.Now;
                clientModel.ApplyTo(client);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteClientAsync(long id)
        {
            var patient = await _context.Clients.FindAsync(id);
            if (patient != null)
            {
                _context.Clients.Remove(patient);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
