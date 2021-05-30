using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UsuariosApi.Models;
using UsuariosApi.DTOS;
using AutoMapper;
using UsuariosApi.Helpers;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace UsuariosApi.Services
{
    public interface IUserService
    {
        UsuarioDTO Authenticate(string username, string password);
        IEnumerable<UsuarioDTO> GetAll();
        UsuarioDTO GetById(int id);
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications


        private readonly AppSettings _appSettings;

        private readonly LoginServiceContext _context;

        private readonly IMapper _mapper;

        public UserService(IOptions<AppSettings> appSettings, LoginServiceContext context, IMapper mapper)
        {
            _appSettings = appSettings.Value;
            _context = context;
            _mapper = mapper;

        }

        public UsuarioDTO Authenticate(string email, string password)
        {
            var user = _context.Usuarios.FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                user = _context.Usuarios.FirstOrDefault(x => x.Email == email + "_" + x.Sal);
                if (user != null) throw new UsuarioInactivoException();
            }

            if (user == null) throw new UsuarioNoAutenticadoException();

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
               password: password,
               salt: Convert.FromBase64String(user.Sal),
               prf: KeyDerivationPrf.HMACSHA1,
               iterationCount: 10000,
               numBytesRequested: 256 / 8));


            if (user.Password != hashed)
                throw new UsuarioNoAutenticadoException();

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Perfil)

                }),
                Expires = DateTime.UtcNow.AddMinutes(90),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var userDTO = _mapper.Map<UsuarioDTO>(user);

            userDTO.Token = tokenHandler.WriteToken(token);

            return userDTO;

        }

        public IEnumerable<UsuarioDTO> GetAll()

        {
            IEnumerable<Usuario> usuarios = _context.Usuarios.ToList();
            return _mapper.Map<IEnumerable<UsuarioDTO>>(usuarios);
        }

        public UsuarioDTO GetById(int id)
        {
            var user = _context.Usuarios.FirstOrDefault(x => x.Id == id);
            return _mapper.Map<UsuarioDTO>(user);

        }
    }

    public class UsuarioInactivoException : System.Exception
    {
        public UsuarioInactivoException() { }
        public UsuarioInactivoException(string message) : base(message) { }
        public UsuarioInactivoException(string message, System.Exception inner) : base(message, inner) { }
        protected UsuarioInactivoException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class UsuarioNoAutenticadoException : System.Exception
    {
        public UsuarioNoAutenticadoException() { }
        public UsuarioNoAutenticadoException(string message) : base(message) { }
        public UsuarioNoAutenticadoException(string message, System.Exception inner) : base(message, inner) { }
        protected UsuarioNoAutenticadoException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}