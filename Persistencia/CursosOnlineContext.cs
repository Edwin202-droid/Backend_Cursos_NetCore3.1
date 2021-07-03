using Dominio;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistencia
{
    public class CursosOnlineContext : IdentityDbContext<Usuario>
    {
        //migracion a sql server
        public CursosOnlineContext(DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   
            //Crear archivos de migracion
            base.OnModelCreating(modelBuilder);

            //Llave compuesta
            modelBuilder.Entity<CursoInstructor>().HasKey(c => new {c.InstructorId, c.CursoId});
        }


        public DbSet<Comentario> Comentario { get; set; }
        public DbSet<Curso> Curso { get; set; }
        public DbSet<CursoInstructor> CursoInstructor { get; set; }
        public DbSet<Instructor> Instructor { get; set; }
        public DbSet<Precio> Precio { get; set; }
        public DbSet<Documento> Documento { get; set; }
   }
}