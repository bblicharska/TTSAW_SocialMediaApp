using Application.Dto;
using Application.Services;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PostApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;
        private readonly ILikeService _likeService;
        private readonly IHttpClientFactory _httpClientFactory;

        public PostController(IPostService postService, ICommentService commentService, ILikeService likeService, IHttpClientFactory httpClientFactory)
        {
            _postService = postService;
            _commentService = commentService;
            _likeService = likeService;
            _httpClientFactory = httpClientFactory;
        }

        // Get all posts
        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<PostDto>> Get()
        {
            var result = _postService.GetAll();
            return Ok(result);
        }

        // Get post by id
        [HttpGet("{id}", Name = "GetPost")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<object>> Get(int id)
        {
            try
            {
                var post = _postService.GetById(id);
                if (post == null)
                {
                    return NotFound();
                }

                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);

                if (!await _postService.IsTokenValidAsync(token))
                {
                    return Unauthorized("Token is not valid");
                }

                var user = await GetUserByIdAsync(post.UserId, token);

                if (user == null)
                {
                    return NotFound("User not found in IdentityService.");
                }

                return Ok(new
                {
                    Post = post,
                    User = user
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // Metoda pomocnicza do pobierania danych użytkownika z Identity API
        private async Task<UserDto> GetUserByIdAsync(int userId, string? token)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync($"http://localhost:8001/User/{userId}");

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
        // Create new post
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Create([FromBody] CreatePostDto dto)
        {
            var id = _postService.Create(dto);
            var actionName = nameof(Get);
            var routeValues = new { id };
            return CreatedAtAction(actionName, routeValues, null);
        }

        // Delete post
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Delete(int id)
        {
            _postService.Delete(id);
            return NoContent();
        }

        // Update post
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Update(int id, [FromBody] UpdatePostDto dto)
        {
            if (id != dto.Id)
            {
                throw new BadRequestException("Id param is not valid");
            }

            _postService.Update(dto);
            return NoContent();
        }

        // Add comment to post
        [HttpPost("{postId}/comments")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult CreateComment(int postId, [FromBody] CreateCommentDto dto)
        {
            var commentId = _commentService.CreateComment(postId, dto);
            var routeValues = new { postId, commentId };
            return CreatedAtAction(nameof(GetCommentsByPostId), routeValues, null);
        }

        // Get comments for a post
        [HttpGet("{postId}/comments")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<object>>> GetCommentsByPostId(int postId)
        {
            var comments = _commentService.GetCommentsByPostId(postId);

            if (comments == null || !comments.Any())
            {
                return NotFound("No comments found for the given post ID.");
            }

            var commentsWithUsers = new List<object>();
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);

            foreach (var comment in comments)
            {
                var user = await GetUserByIdAsync(comment.UserId, token);
                commentsWithUsers.Add(new
                {
                    Comment = comment,
                    User = user
                });
            }

            return Ok(commentsWithUsers);
        }

        // Delete comment
        [HttpDelete("comments/{commentId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteComment(int commentId)
        {
            _commentService.DeleteComment(commentId);
            return NoContent();
        }

        // Add like to post
        [HttpPost("{postId}/likes")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult AddLike(int postId, [FromBody] CreateLikeDto dto)
        {
            _likeService.AddLike(postId, dto);
            return Ok();
        }

        // Remove like from post
        [HttpDelete("{postId}/likes/{likeId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult RemoveLike(int postId, int likeId)
        {
            _likeService.RemoveLike(likeId);
            return Ok();
        }

        // Get likes for post
        [HttpGet("{postId}/likes")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<object>>> GetLikesByPostId(int postId)
        {
            var likes = _likeService.GetLikesByPostId(postId);

            if (likes == null || !likes.Any())
            {
                return NotFound("No likes found for the given post ID.");
            }

            var likesWithUsers = new List<object>();
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);

            foreach (var like in likes)
            {
                var user = await GetUserByIdAsync(like.UserId, token);
                likesWithUsers.Add(new
                {
                    Like = like,
                    User = user
                });
            }

            return Ok(likesWithUsers);
        }

        // Count likes for post
        [HttpGet("{postId}/likes/count")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<int> CountLikes(int postId)
        {
            var likeCount = _likeService.CountLikesByPostId(postId);
            return Ok(likeCount);
        }

        // Check if a post is liked by a specific user
        [HttpGet("{postId}/likes/{userId}/check")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<bool> IsPostLikedByUser(int postId, int userId)
        {
            var isLiked = _likeService.IsPostLikedByUser(postId, userId);
            return Ok(isLiked);
        }
    }

}
