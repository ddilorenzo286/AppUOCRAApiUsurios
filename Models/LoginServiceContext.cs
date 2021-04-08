using Microsoft.EntityFrameworkCore;

namespace UsuariosApi.Models
{
    public class LoginServiceContext : DbContext
    {
        public LoginServiceContext(DbContextOptions<LoginServiceContext> options)
            : base(options)
        {
        }
        public DbSet<Usuario> Usuarios { get; set; }

    }
}