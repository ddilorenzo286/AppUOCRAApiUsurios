using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UsuariosApi.Models;
using UsuariosApi.DTOS;
using UsuariosApi.Entities;
using Microsoft.AspNetCore.JsonPatch;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace LoginService.Controllers
{
    public class UsuarioQueryController : ControllerBase
    {

        private readonly LoginServiceContext _context;
        private readonly IMapper _mapper;
        public UsuarioQueryController(LoginServiceContext context, IMapper mapper, ILogger<UsuarioController> logger)
        {
            _context = context;
            _mapper = mapper;

        }

        [EnableQuery]
        [Authorize(Roles = Roles.Admin)]
        public IEnumerable<UsuarioDTO> Get()
        {
            IEnumerable<Usuario> usuarios = _context.Usuarios.AsQueryable().ToList();
            return _mapper.Map<IEnumerable<UsuarioDTO>>(usuarios).ToList();
        }
    }
}