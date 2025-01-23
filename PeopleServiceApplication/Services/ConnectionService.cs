using AutoMapper;
using PeopleServiceApplication.Dto;
using PeopleServiceDomain.Contracts;
using PeopleServiceDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PeopleServiceApplication.Services
{
    public class ConnectionService : IConnectionService
    {
        private readonly IConnectionUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly string _identityApiUrl = "https://localhost:7132"; // URL Identity API

        public ConnectionService(IConnectionUnitOfWork uow, IMapper mapper, HttpClient httpClient)
        {
            _uow = uow;
            _mapper = mapper;
            this._httpClient = httpClient;
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_identityApiUrl}/User/validate-token"); // Endpoint weryfikacji tokenu
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);

            return response.IsSuccessStatusCode;
        }

        public async Task<ConnectionDto> GetConnectionAsync(int id)
        {
            var connection = await _uow.ConnectionRepository.GetByIdAsync(id);
            return connection != null ? _mapper.Map<ConnectionDto>(connection) : null;
        }

        public async Task<IEnumerable<ConnectionDto>> GetUserConnectionsAsync(int userId)
        {
            var connections = await _uow.ConnectionRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<ConnectionDto>>(connections);
        }

        public async Task<ConnectionDto> CreateConnectionAsync(CreateConnectionDto dto)
        {
            var connection = _mapper.Map<Connection>(dto);
            connection.CreatedAt = DateTime.UtcNow;

            await _uow.ConnectionRepository.AddAsync(connection);
            _uow.Commit();

            return _mapper.Map<ConnectionDto>(connection);
        }

        public async Task<bool> DeleteConnectionAsync(int id)
        {
            var connection = await _uow.ConnectionRepository.GetByIdAsync(id);
            if (connection == null) return false;

            await _uow.ConnectionRepository.DeleteAsync(connection);
            _uow.Commit();

            return true;
        }

        public async Task<ConnectionDto> UpdateConnectionAsync(int id, UpdateConnectionDto dto)
        {
            var connection = await _uow.ConnectionRepository.GetByIdAsync(id);
            if (connection == null) return null;

            connection.FriendId = dto.FriendId;

            await _uow.ConnectionRepository.UpdateAsync(connection);
            _uow.Commit();

            return _mapper.Map<ConnectionDto>(connection);
        }
    }
}
