using IdentityServiceApplication.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServiceApplication.Services
{
    public interface IUserService
    {
        TokenDto Register(RegisterUserDto registerUserDto);
        TokenDto Login(LoginUserDto loginUserDto);
        UserDto GetUserById(int userId);
        List<UserDto> GetAll();
        void Delete(int id);
        void UpdateUser(int userId, UpdateUserDto updateUserDto);
        void ChangePassword(int userId, ChangePasswordDto changePasswordDto);
    }
}
