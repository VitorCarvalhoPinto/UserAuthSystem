using BCrypt;
using Application.DTOs;
using Application.Interfaces;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class UserService: IUserService
    {
        IUserRepository _userRepository;
        IMapper _mapper;
        IJwtService _jwtService;
        public UserService(IUserRepository userRepository, IMapper mapper, JwtService jwtService) 
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _jwtService = jwtService;
        }
        public async Task<UserCreateDTO> UserCreate(UserCreateDTO user)
        {
            var userEntity = new User
            {
                Email = user.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash),
                Name = user.Name,
            };
            return _mapper.Map<UserCreateDTO>(_userRepository.CreateAsync(userEntity));
        }

        public async Task<string> UserLogin(UserLoginDTO user)
        {
            var userEntity = await _userRepository.FindByEmailAsync(user.Email);
            if (userEntity == null)
            {
                throw new Exception("Usuário não encontrado");
            }
            if (!BCrypt.Net.BCrypt.Verify(user.Password, userEntity.PasswordHash))
            {
                throw new Exception("Credenciais inválidas");
            }
            return _jwtService.GenerateToken(userEntity);
        }
    }
}
