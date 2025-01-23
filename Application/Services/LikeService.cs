using Application.Dto;
using AutoMapper;
using Domain.Contracts;
using Domain.Exceptions;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class LikeService : ILikeService
    {
        private readonly IPostUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LikeService(IPostUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void AddLike(int postId, CreateLikeDto dto)
        {
            if (dto == null)
            {
                throw new BadRequestException("Like data is null");
            }

            // Sprawdzenie, czy użytkownik już polubił post
            var existingLike = _unitOfWork.LikeRepository.GetAll()
                .FirstOrDefault(l => l.PostId == postId && l.UserId == dto.UserId);

            if (existingLike != null)
            {
                throw new BadRequestException("Post is already liked by this user");
            }

            var like = _mapper.Map<Like>(dto);
            like.CreatedAt = DateTime.Now;
            like.PostId = postId; // przypisz postId do polubienia

            _unitOfWork.LikeRepository.Insert(like);
            _unitOfWork.Commit();
        }

        public void RemoveLike(int likeId)
        {
            var like = _unitOfWork.LikeRepository.Get(likeId);
            if (like == null)
            {
                throw new NotFoundException("Like not found");
            }

            _unitOfWork.LikeRepository.Delete(like);
            _unitOfWork.Commit();
        }

        public List<LikeDto> GetLikesByPostId(int postId)
        {
            var post = _unitOfWork.PostRepository.Get(postId);
            if (post == null)
            {
                throw new NotFoundException("Post not found");
            }

            var likes = _unitOfWork.LikeRepository.GetAll()
                .Where(l => l.PostId == postId)
                .ToList();

            return _mapper.Map<List<LikeDto>>(likes);
        }

        public int CountLikesByPostId(int postId)
        {
            return _unitOfWork.LikeRepository.GetAll()
                .Count(l => l.PostId == postId);
        }

        public bool IsPostLikedByUser(int postId, int userId)
        {
            return _unitOfWork.LikeRepository.GetAll()
                .Any(l => l.PostId == postId && l.UserId == userId);
        }
    }

}
