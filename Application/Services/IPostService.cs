using Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IPostService
    {
        List<PostDto> GetAll();
        Task<bool> IsTokenValidAsync(string? token);
        PostDto GetById(int id);
        int Create(CreatePostDto dto);
        void Update(UpdatePostDto dto);
        void Delete(int id);
    }
}
