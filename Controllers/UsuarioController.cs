
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsuariosApi.Models;
using UsuariosApi.DTOS;
using UsuariosApi.Entities;
using Microsoft.AspNetCore.JsonPatch;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using mails;

namespace LoginService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly LoginServiceContext _context;
        private readonly IMapper _mapper;

        private readonly IMailService _mailService;

        public UsuarioController(LoginServiceContext context, IMapper mapper, ILogger<UsuarioController> logger, IMailService mailService)
        {
            _context = context;
            _mapper = mapper;
            _mailService = mailService;
        }


        // PUT: api/Usuario/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult> PutUsuario(int id, [FromBody] Usuario usuario)
        {



            if (id != usuario.Id)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // PUT: api/Usuario/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPatch("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult> PatchUsuario(int id, [FromBody] JsonPatchDocument<Usuario> usuarioPatch)
        {


            Usuario usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return NotFound();
            }

            try
            {
                usuarioPatch.ApplyTo(usuario);

                _context.Entry(usuario).State = EntityState.Modified;

                await _context.SaveChangesAsync();


            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // POST: api/Usuario
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[Authorize(Roles = Roles.Admin)]
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult> PostUsuario(Usuario usuario)
        {

            if (_context.Usuarios.Any(e => e.Email == usuario.Email))
                return BadRequest();

            usuario.Ticket = Guid.NewGuid().ToString();

            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            usuario.Sal = Convert.ToBase64String(salt);
            usuario.Password = "";
            usuario.FechaCreacion = DateTime.UtcNow;
            usuario.Activo = true;

            _context.Usuarios.Add(usuario);

            await _context.SaveChangesAsync();

            await _mailService.newUserMail(usuario.Perfil, usuario.Ticket, usuario.Email, usuario.Nombre + "," + usuario.Apellido);

            return Ok();
        }



        // DELETE: api/Usuario/5
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }

    }

}
