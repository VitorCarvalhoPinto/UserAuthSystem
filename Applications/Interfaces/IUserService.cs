using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDTO> UserCreate(UserCreateDTO user);
        Task<UserLoginResponseDTO> UserLogin(UserLoginDTO user);
    }
}
