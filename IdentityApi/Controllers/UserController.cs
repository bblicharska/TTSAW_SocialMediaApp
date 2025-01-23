using IdentityServiceApplication.Dto;
using IdentityServiceApplication.Services;
using IdentityServiceDomain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserDto>> Get()
        {
            var result = _userService.GetAll();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult Delete(int id)
        {
            _userService.Delete(id);
            return NoContent();
        }

        // Rejestracja nowego użytkownika
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterUserDto registerUserDto)
        {
            try
            {
                var tokenDto = _userService.Register(registerUserDto);
                return Ok(tokenDto); // Zwracamy token w odpowiedzi
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message); // Zwracamy błąd, jeśli coś poszło nie tak
            }
        }

        // Logowanie użytkownika
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginUserDto loginUserDto)
        {
            try
            {
                var tokenDto = _userService.Login(loginUserDto);
                return Ok(tokenDto); // Zwracamy token w odpowiedzi
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message); // Zwracamy błąd, jeśli dane są niepoprawne
            }
        }

        // Pobieranie danych użytkownika po ID
        [HttpGet("{userId}")]
        [Authorize]
        public IActionResult GetUserById(int userId)
        {
            try
            {
                var userDto = _userService.GetUserById(userId);
                return Ok(userDto); // Zwracamy dane użytkownika
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message); // Zwracamy 404, jeśli użytkownik nie istnieje
            }
        }

        // Aktualizowanie danych użytkownika
        [HttpPut("{userId}")]
        [Authorize]
        public IActionResult UpdateUser(int userId, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                _userService.UpdateUser(userId, updateUserDto);
                return NoContent(); // Zwracamy 204 No Content po udanej aktualizacji
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message); // Zwracamy 404, jeśli użytkownik nie istnieje
            }
        }

        // Zmiana hasła użytkownika
        [HttpPut("{userId}/change-password")]
        [Authorize]
        public IActionResult ChangePassword(int userId, [FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                _userService.ChangePassword(userId, changePasswordDto);
                return NoContent(); // Zwracamy 204 No Content po udanej zmianie hasła
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message); // Zwracamy 404, jeśli użytkownik nie istnieje
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message); // Zwracamy 400, jeśli dane są niepoprawne
            }
        }

        [Route("validate-token")]
        [HttpGet]
        [Authorize]
        public IActionResult ValidateToken()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Pobranie ID użytkownika z tokenu

            if (userId == null)
            {
                return Unauthorized("Invalid token"); // Zwracamy 401, jeśli token jest nieważny
            }

            return Ok(new { Message = "Token is valid" }); // Token jest ważny
        }
    }
}
