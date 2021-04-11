using System;
using System.Text;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsuariosApi.Models;
using UsuariosApi.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using UsuariosApi.DTOS;
using UsuariosApi.Entities;
using AutoMapper;
using RabbitMQ.Client;
using mails;
using System.Reflection;

namespace LoginService.Controllers
{
    [Route("api/Autorizacion")]
    [ApiController]
    [Authorize]
    public class AutorizacionController : ControllerBase
    {
        private readonly LoginServiceContext _context;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        private readonly IgoogleMailService _mailService;

        public AutorizacionController(LoginServiceContext context, IUserService userService, IMapper mapper, IgoogleMailService mailService)
        {
            _context = context;
            _userService = userService;
            _mapper = mapper;
            _mailService = mailService;
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult login([FromBody] Credencial credencial)

        {
            var user = _userService.Authenticate(credencial.email, credencial.password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

        [HttpPost("recupero")]
        [AllowAnonymous]
        public async Task<ActionResult> recupero([FromBody] string email)

        {

            Usuario user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return NotFound();

            user.Ticket = Guid.NewGuid().ToString();

            _context.SaveChanges();

            _mailService.passwordMail(user);

            return Ok();

        }

        [HttpPost("renovacion")]
        [AllowAnonymous]
        public async Task<ActionResult> renovacion([FromBody] CredencialRenovacion credencial)

        {

            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Ticket == credencial.ticket);

            if (user == null)
                return NotFound();


            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: credencial.password,
                salt: Convert.FromBase64String(user.Sal),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            user.Password = hashed;

            user.Ticket = "";

            _context.SaveChanges();

            return Ok();

        }



        [HttpPost("logon")]
        [AllowAnonymous]
        public async Task<ActionResult<UsuarioDTO>> logon([FromBody] LogonCommand comando)
        {

            if (await _context.Usuarios.AnyAsync(e => e.Email == comando.Email))
                return BadRequest();

            Usuario usuario = new Usuario();

            usuario.Ticket = Guid.NewGuid().ToString();

            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            usuario.Sal = Convert.ToBase64String(salt);
            usuario.Password = "";
            usuario.FechaCreacion = DateTime.UtcNow;
            usuario.Nombre = comando.Nombre;
            usuario.Apellido = comando.Apellido;
            usuario.Email = comando.Email;
            usuario.Foto = comando.Foto;
            usuario.Documento = comando.Documento;
            usuario.TipoDocumento = comando.TipoDocumento;
            usuario.Perfil = Roles.Cliente;
            usuario.Telefono = comando.Telefono;
            usuario.Activo = true;

            _context.Usuarios.Add(usuario);

            await _context.SaveChangesAsync();

            //await _mailService.newUserMail(usuario.Perfil, usuario.Ticket, usuario.Email, usuario.Nombre + "," + usuario.Apellido);

            _mailService.logonMail(usuario);





            return _mapper.Map<UsuarioDTO>(usuario); ;
        }

        [HttpPost("updateProfile")]
        [AllowAnonymous]
        public async Task<ActionResult<UsuarioDTO>> updateProfile([FromBody] updateProfileCommand comando)
        {


            System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            int id;

            var res = int.TryParse(currentUser.Identity.Name, out id);



            Usuario user = await _context.Usuarios.FirstOrDefaultAsync(e => e.Id == id);

            if (user == null)
                return BadRequest();

            user.Nombre = comando.Nombre != null ? comando.Nombre : user.Nombre;
            user.Apellido = comando.Apellido != null ? comando.Apellido : user.Apellido;
            user.Foto = comando.Foto != null ? comando.Foto : user.Foto;
            user.Documento = comando.Documento != 0 ? comando.Documento : user.Documento;
            user.TipoDocumento = comando.TipoDocumento != null ? comando.TipoDocumento : user.TipoDocumento;
            user.Telefono = comando.Telefono != null ? comando.Telefono : user.Telefono;
            user.FechaNacimiento = comando.FechaNacimiento != null ? comando.FechaNacimiento : user.FechaNacimiento;
            user.LugarResidencia = comando.LugarResidencia != null ? comando.LugarResidencia : user.LugarResidencia;
            user.Sexo = comando.Sexo != null ? comando.Sexo : user.Sexo;

            await _context.SaveChangesAsync();

            return _mapper.Map<UsuarioDTO>(user);

        }



    }


    public class Credencial
    {

        public string email { get; set; }
        public string password { get; set; }

    }

    public class CredencialRenovacion
    {

        public string ticket { get; set; }
        public string password { get; set; }

    }


    public class updateProfileCommand
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int Documento { get; set; }
        public string TipoDocumento { get; set; }
        public string Telefono { get; set; }
        public string Foto { get; set; }
        public string Sexo { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string LugarResidencia { get; set; }

    }

    public class LogonCommand
    {
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int Documento { get; set; }
        public string TipoDocumento { get; set; }
        public string Telefono { get; set; }
        public string Foto { get; set; }
        public int InvitacionId { get; set; }
    }

}
