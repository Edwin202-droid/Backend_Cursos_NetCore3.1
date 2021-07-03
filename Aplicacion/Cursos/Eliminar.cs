using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using MediatR;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Eliminar
    {
        public class Ejecuta : IRequest
        {
            public Guid Id { get; set; }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CursosOnlineContext context;

            public Manejador(CursosOnlineContext context)
            {
                this.context = context;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Lista de instructores del curso
                var instructoresDB = context.CursoInstructor.Where(x => x.CursoId == request.Id);
                foreach (var instructor in instructoresDB)
                {
                    context.CursoInstructor.Remove(instructor);
                }

                //Obtener comentarios para eliminar
                var comentariosDB = context.Comentario.Where( x => x.CursoId == request.Id);
                foreach (var cmt in comentariosDB)
                {
                    context.Comentario.Remove(cmt);
                }
                //Obtener precios para eliminar
                var precioDB = context.Precio.Where( x => x.CursoId == request.Id).FirstOrDefault();
                if(precioDB!=null)
                {
                    context.Precio.Remove(precioDB);
                }

                var curso = await context.Curso.FindAsync(request.Id);
                if(curso==null)
                {
                    //throw new Exception("No se puede eliminar curso");
                    //Usamos nuestro Manejador error, donde puedes introducir el codigo del error y un mensaje
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new {mensaje= "No se encontro el curso"});
                }

                context.Remove(curso);
                var resultado = await context.SaveChangesAsync();

                if(resultado > 0)
                {
                    return Unit.Value;
                }
                throw new Exception("No se pudieron guardar los cambios");
            }
        }
    }
}