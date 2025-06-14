using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using BCrypt;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {
        IUserRepository _userRepository;
        IMapper _mapper;
        IJwtService _jwtService;
        public UserService(IUserRepository userRepository, IMapper mapper, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _jwtService = jwtService;
        }
        public async Task<UserResponseDTO> UserCreate(UserCreateDTO user)
        {
            var emailChecker = new EmailAddressAttribute();
            var existingUser = await _userRepository.FindByEmailAsync(user.Email);
            if (existingUser != null)
            {
                throw new ValidationException("Usuário com este email já existe");
            }

            // Validação básica de senha
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                throw new ValidationException("Senha não pode estar vazia");
            }

            if (user.PasswordHash.Length < 6)
            {
                throw new ValidationException("Senha deve ter pelo menos 6 caracteres");
            }

            // Validação de email
            if (string.IsNullOrWhiteSpace(user.Email) || !emailChecker.IsValid(user.Email))
            {
                throw new ValidationException("Email deve ter um formato válido");
            }

            // Validação de nome
            if (string.IsNullOrWhiteSpace(user.Name) || user.Name.Length < 2)
            {
                throw new ValidationException("Nome deve ter pelo menos 2 caracteres");
            }

            var userEntity = new User
            {
                Email = user.Email.ToLower().Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash),
                Name = user.Name.Trim(),
            };

            var createdUser = await _userRepository.CreateAsync(userEntity);
            return _mapper.Map<UserResponseDTO>(createdUser);
        }

        public async Task<UserLoginResponseDTO> UserLogin(UserLoginDTO user)
        {
            // Validação de entrada
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new ValidationException("Email é obrigatório");
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                throw new ValidationException("Senha é obrigatória");
            }

            var userEntity = await _userRepository.FindByEmailAsync(user.Email.ToLower().Trim());
            if (userEntity == null)
            {
                throw new ArgumentException("Credenciais inválidas");
            }

            if (!BCrypt.Net.BCrypt.Verify(user.Password, userEntity.PasswordHash))
            {
                throw new ArgumentException("Credenciais inválidas");
            }

            return new UserLoginResponseDTO
            {
                Id = userEntity.Id,
                Name = userEntity.Name,
                Email = userEntity.Email,
                Token = _jwtService.GenerateToken(userEntity)
            };
    }
    }
}
