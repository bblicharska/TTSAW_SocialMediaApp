using AutoMapper;
using IdentityServiceApplication.Dto;
using IdentityServiceDomain.Contracts;
using IdentityServiceDomain.Exceptions;
using IdentityServiceDomain.Models;

namespace IdentityServiceApplication.Services
{
    public class UserService : IUserService
    {
        private readonly IUserUnitOfWork _uow; // Używamy IUserUnitOfWork do zarządzania repozytoriami
        private readonly IPasswordHasher _passwordHasher; // Hashowanie hasła
        private readonly IJwtTokenGenerator _jwtTokenGenerator; // Generowanie tokenu JWT
        private readonly IMapper _mapper; // Mapper do mapowania obiektów

        public UserService(
            IUserUnitOfWork uow,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            IMapper mapper)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _mapper = mapper;
        }

        public TokenDto Register(RegisterUserDto registerUserDto)
        {
            if (registerUserDto == null)
            {
                throw new BadRequestException("User data is null");
            }

            // Sprawdzamy, czy użytkownik już istnieje
            if (_uow.UserRepository.UserExists(registerUserDto.Email))
            {
                throw new BadRequestException("A user with this email already exists.");
            }

            if (registerUserDto.Password != registerUserDto.ConfirmPassword)
            {
                throw new BadRequestException("Passwords do not match.");
            }

            // Tworzymy nowego użytkownika
            var user = new User
            {
                Username = registerUserDto.Username,
                Email = registerUserDto.Email,
                PasswordHash = _passwordHasher.HashPassword(registerUserDto.Password),
                Role = "User", // Domyślna rola
                CreatedAt = DateTime.UtcNow
            };

            // Dodajemy użytkownika do repozytorium
            _uow.UserRepository.Insert(user);
            _uow.Commit(); // Zapisujemy zmiany w bazie danych

            // Generate token
            var (token, expirationDate) = _jwtTokenGenerator.GenerateToken(user);

            return new TokenDto
            {
                AccessToken = token,
                ExpiresAt = expirationDate
            };
        }

        public TokenDto Login(LoginUserDto loginUserDto)
        {
            if (loginUserDto == null || string.IsNullOrWhiteSpace(loginUserDto.UsernameOrEmail) || string.IsNullOrWhiteSpace(loginUserDto.Password))
            {
                throw new BadRequestException("Invalid login data.");
            }

            // Normalize input for case-insensitivity
            var normalizedInput = loginUserDto.UsernameOrEmail.Trim().ToLower();

            // Retrieve user
            var user = _uow.UserRepository.GetByUsernameOrEmail(normalizedInput);
            if (user == null || !_passwordHasher.VerifyPassword(user.PasswordHash, loginUserDto.Password))
            {
                throw new BadRequestException("Invalid credentials.");
            }

            // Generate token
            var (token, expirationDate) = _jwtTokenGenerator.GenerateToken(user);

            return new TokenDto
            {
                AccessToken = token,
                ExpiresAt = expirationDate
            };
        }


        public List<UserDto> GetAll()
        {
            var users = _uow.UserRepository.GetAll().ToList();

            // Mapowanie User na UserDto
            List<UserDto> result = _mapper.Map<List<UserDto>>(users);
            return result;
        }

        public UserDto GetUserById(int userId)
        {
            var user = _uow.UserRepository.GetById(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            return _mapper.Map<UserDto>(user); // Mapowanie User na UserDto
        }

        public void Delete(int id)
        {
            var post = _uow.UserRepository.Get(id);
            if (post == null)
            {
                throw new NotFoundException("Post not found");
            }

            _uow.UserRepository.Delete(post);
            _uow.Commit();
        }

        public void UpdateUser(int userId, UpdateUserDto updateUserDto)
        {
            if (updateUserDto == null)
            {
                throw new BadRequestException("User data is null");
            }

            var user = _uow.UserRepository.GetById(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            user.Username = updateUserDto.Username;
            user.Email = updateUserDto.Email;

            _uow.UserRepository.Update(user);
            _uow.Commit(); // Zapisanie zmian w bazie
        }

        public void ChangePassword(int userId, ChangePasswordDto changePasswordDto)
        {
            if (changePasswordDto == null)
            {
                throw new BadRequestException("Password change data is null");
            }

            var user = _uow.UserRepository.GetById(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            if (!_passwordHasher.VerifyPassword(user.PasswordHash, changePasswordDto.CurrentPassword))
            {
                throw new BadRequestException("Current password is incorrect.");
            }

            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
            {
                throw new BadRequestException("New passwords do not match.");
            }

            user.PasswordHash = _passwordHasher.HashPassword(changePasswordDto.NewPassword);

            _uow.UserRepository.Update(user);
            _uow.Commit(); // Zapisanie zmian w bazie
        }
    }

}

