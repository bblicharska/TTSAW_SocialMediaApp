using Microsoft.AspNetCore.Mvc;
using PeopleServiceApplication.Dto;
using PeopleServiceApplication.Services;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace PeopleApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ConnectionsController : ControllerBase
    {
        private readonly IConnectionService _connectionService;
        private readonly IHttpClientFactory _httpClientFactory;


        public ConnectionsController(IConnectionService connectionService, IHttpClientFactory httpClientFactory)
        {
            _connectionService = connectionService;
            _httpClientFactory = httpClientFactory;
        }

        // GET api/connections/{id}
        [HttpGet("{id}", Name = "GetConnection")]
        [Authorize]
        public async Task<ActionResult<object>> GetConnection(int id)
        {
            try
            {
                var connection = await _connectionService.GetConnectionAsync(id);
                if (connection == null)
                {
                    return NotFound();
                }

                // Pobieramy token z nagłówka autoryzacji
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);

                // Sprawdzamy, czy token jest ważny
                if (!await _connectionService.IsTokenValidAsync(token))
                {
                    return Unauthorized("Token is not valid");
                }

                // Pobieramy użytkownika i przyjaciela z Identity API
                var user = await GetUserByIdAsync(connection.UserId, token);
                var friend = await GetUserByIdAsync(connection.FriendId, token);

                if (user == null)
                {
                    return NotFound("User not found in IdentityService.");
                }

                if (friend == null)
                {
                    return NotFound("Friend not found in IdentityService.");
                }

                // Zwracamy połączenie i dane użytkowników
                return Ok(new
                {
                    Connection = connection,
                    User = user,
                    Friend = friend
                });
            }
            catch (Exception ex)
            {
                // Obsługa błędów wewnętrznych
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        private async Task<UserDto> GetUserByIdAsync(int userId, string? token)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync($"https://localhost:7132/User/{userId}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error fetching user: {response.StatusCode}");
                    return null;
                }

                var userJson = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<UserDto>(userJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetUserByIdAsync: {ex.Message}");
                return null;
            }
        }

        // GET api/connections/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserConnections(int userId)
        {
            var connections = await _connectionService.GetUserConnectionsAsync(userId);
            if (connections == null || connections.GetEnumerator().MoveNext() == false)
            {
                return NotFound(new { Message = $"No connections found for user with ID {userId}." });
            }

            return Ok(connections);
        }

        // POST api/connections
        [HttpPost]
        public async Task<IActionResult> CreateConnection([FromBody] CreateConnectionDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { Message = "Invalid connection data." });
            }

            var connection = await _connectionService.CreateConnectionAsync(dto);

            return CreatedAtAction(nameof(GetConnection), new { id = connection.Id }, connection);
        }

        // DELETE api/connections/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConnection(int id)
        {
            var success = await _connectionService.DeleteConnectionAsync(id);
            if (!success)
            {
                return NotFound(new { Message = $"Connection with ID {id} not found." });
            }

            return NoContent();
        }

        // PUT api/connections/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateConnection(int id, [FromBody] UpdateConnectionDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { Message = "Invalid data for updating the connection." });
            }

            var connection = await _connectionService.UpdateConnectionAsync(id, dto);
            if (connection == null)
            {
                return NotFound(new { Message = $"Connection with ID {id} not found." });
            }

            return Ok(connection);
        }
    }
}
