using AutoMapper;
using Domain.Contracts;
using Domain.Exceptions;
using Domain.Models;
using Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class PostService : IPostService
    {
        private readonly IPostUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly string _identityApiUrl = "http://localhost:8001"; // URL Identity API

        public PostService(IPostUnitOfWork unitOfWork, IMapper mapper, HttpClient httpClient)
        {
            this._uow = unitOfWork;
            this._mapper = mapper;
            this._httpClient = httpClient;
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_identityApiUrl}/User/validate-token"); // Endpoint weryfikacji tokenu
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);

            return response.IsSuccessStatusCode;
        }

        public int Create(CreatePostDto dto)
        {
            if (dto == null)
            {
                throw new BadRequestException("Post data is null");
            }

            var createdDate = DateTime.Now;
            var post = _mapper.Map<Post>(dto);
            post.CreatedAt = createdDate;

            post.ImageUrl = String.IsNullOrEmpty(dto.ImageUrl)
                ? ""
                : dto.ImageUrl;

            _uow.PostRepository.Insert(post);
            _uow.Commit();

            return post.Id;
        }

        public void Delete(int id)
        {
            var post = _uow.PostRepository.Get(id);
            if (post == null)
            {
                throw new NotFoundException("Post not found");
            }

            _uow.PostRepository.Delete(post);
            _uow.Commit();
        }

        public List<PostDto> GetAll()
        {
            var posts = _uow.PostRepository.GetAll().ToList(); 

            List<PostDto> result = _mapper.Map<List<PostDto>>(posts);
            return result;
        }

        public PostDto GetById(int id)
        {
            if (id <= 0)
            {
                throw new BadRequestException("Id is less than zero");
            }

            var post = _uow.PostRepository.GetById(id);
            if (post == null)
            {
                throw new NotFoundException("Post not found");
            }

            var result = _mapper.Map<PostDto>(post);
            return result;
        }


        public void Update(UpdatePostDto dto)
        {
            if (dto == null)
            {
                throw new BadRequestException("No post data provided");
            }

            var post = _uow.PostRepository.Get(dto.Id);
            if (post == null)
            {
                throw new NotFoundException("Post not found");
            }

            post.UserId = dto.UserId;
            post.Content = dto.Content;
            post.ImageUrl = String.IsNullOrEmpty(dto.ImageUrl)
                ? ""
                : dto.ImageUrl;

            _uow.Commit();
        }
    }
}
