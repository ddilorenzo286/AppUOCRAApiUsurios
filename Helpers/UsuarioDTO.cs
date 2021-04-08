using System;
namespace UsuariosApi.DTOS
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public int Documento { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Perfil { get; set; }
        public string Foto { get; set; }
        public string Token { get; set; }
        public string Ticket { get; set; }
        public string Telefono { get; set; }
        public bool Activo { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public string Sexo { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string LugarResidencia { get; set; }

    }

}