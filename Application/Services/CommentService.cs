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
    public class CommentService : ICommentService
    {
        private readonly IPostUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CommentService(IPostUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public int CreateComment(int postId, CreateCommentDto dto)
        {
            if (dto == null)
            {
                throw new BadRequestException("Comment data is null");
            }

            // Sprawdzenie, czy post istnieje
            var post = _unitOfWork.PostRepository.Get(postId);
            if (post == null)
            {
                throw new NotFoundException("Post not found");
            }

            var comment = _mapper.Map<Comment>(dto);
            comment.CreatedAt = DateTime.Now;
            comment.PostId = postId; // przypisz postId do komentarza

            _unitOfWork.CommentRepository.Insert(comment);
            _unitOfWork.Commit();

            return comment.Id; // Zwrócenie ID utworzonego komentarza
        }

        public void DeleteComment(int commentId)
        {
            var comment = _unitOfWork.CommentRepository.Get(commentId);
            if (comment == null)
            {
                throw new NotFoundException("Comment not found");
            }

            _unitOfWork.CommentRepository.Delete(comment);
            _unitOfWork.Commit();
        }

        public List<CommentDto> GetCommentsByPostId(int postId)
        {
            var post = _unitOfWork.PostRepository.Get(postId);
            if (post == null)
            {
                throw new NotFoundException("Post not found");
            }

            var comments = _unitOfWork.CommentRepository.GetAll()
                .Where(c => c.PostId == postId)
                .ToList();

            return _mapper.Map<List<CommentDto>>(comments);
        }
    }

}
