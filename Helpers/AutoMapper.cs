using AutoMapper;
using UsuariosApi.Models;
using UsuariosApi.DTOS;
namespace UsuariosApi.Helpers
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Usuario, UsuarioDTO>();
        }
    }
}