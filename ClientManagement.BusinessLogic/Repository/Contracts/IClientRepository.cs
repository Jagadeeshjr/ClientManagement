using ClientManagement.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace ClientManagement.BusinessLogic.Repository.Contracts
{
    public interface IClientRepository
    {
        Task<PagedClientResult> GetAllclientsBySortingAsync(string term, string? sort, int page, int limit);

        Task<List<Client>> GetAllClientsAsync();

        Task<Client> GetClientByIdAsync(long id);

        Task<bool> PatientExistsAsync(long id);

        Task<long> AddClientAsync(Client clientModel);

        Task UpdateClientAsync(Client clientModel);

        Task UpdateClientPatchAsync(long id, JsonPatchDocument clientModel);

        Task<bool> DeleteClientAsync(long id);
    }
}