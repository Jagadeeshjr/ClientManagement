using ClientManagement.Models;
using ClientManagement.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace ClientManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;

        public ClientController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllClients()
        {
            var clients = await _clientRepository.GetAllClientsAsync();

            if (clients == null || clients.Count == 0)
            {
                return NotFound();
            }
            return Ok(clients);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllClientsByFiltering(string? term, string? sort, int page = 1, int limit = 10)
        {

            var filterRecords = await _clientRepository.GetAllclientsBySortingAsync(term, sort, page, limit);

            if (filterRecords.Clients == null)
            {
                return NotFound();
            }

            return Ok(filterRecords.Clients);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetClientById([FromRoute] long id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var client = await _clientRepository.GetClientByIdAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return Ok(client);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewClient([FromBody] Client clientmodel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var id = await _clientRepository.AddClientAsync(clientmodel);

            return CreatedAtAction(nameof(GetClientById), new { id, controller = "client" }, id);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient([FromRoute] long id, [FromBody] Client clientmodel)
        {

            if (id <= 0 || id != clientmodel.ClientId)
            {
                return BadRequest();
            }

            if (!await _clientRepository.PatientExistsAsync(id))
            {
                return NotFound();
            }

            await _clientRepository.UpdateClientAsync(clientmodel);
            return Ok();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateClientPatch([FromRoute] long id, [FromBody] JsonPatchDocument clientModel)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var client = await _clientRepository.GetClientByIdAsync(id);

            if (client == null)
            {
                return NotFound();
            }
            await _clientRepository.UpdateClientPatchAsync(id, clientModel);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient([FromRoute] long id)
        {
            var client = await _clientRepository.DeleteClientAsync(id);

            if (client != true)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
