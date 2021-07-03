using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Dominio;
using FluentValidation;
using MediatR;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Nuevo
    {
        public class Ejecuta : IRequest
        {   
            //Los elementos que necesito para crear el CURSO
            //Aqui ponemos los required
            //[Required(ErrorMessage="Por favor ingrese el titulo del curso")]
            public string Titulo { get; set; }
            public string Descripcion { get; set; }
            public DateTime? FechaPublicacion { get; set; }

            public List<Guid> ListaInstructor { get; set; }
            public decimal Precio { get; set; }
            public decimal Promocion { get; set; }
        }

        //Logica de Validaciones con Fluent
        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            public EjecutaValidacion()
            {
                RuleFor(x => x.Titulo).NotEmpty(); //El titulo no permite valores vacios
                RuleFor(x => x.Descripcion).NotEmpty();
                RuleFor(x => x.FechaPublicacion).NotEmpty();
            }
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
                //Creamos el id del curso, con un guid
                Guid _cursoId = Guid.NewGuid();
                var curso = new Curso {
                    CursoId= _cursoId,
                    Titulo = request.Titulo,
                    Descripcion = request.Descripcion,
                    FechaPublicacion = request.FechaPublicacion,
                    FechaCreacion = DateTime.UtcNow
                };

                context.Curso.Add(curso);

                //Insertar en cursoinstructor
                if(request.ListaInstructor != null)
                {

                    foreach (var id in request.ListaInstructor)
                    {
                        var cursoInstructor = new CursoInstructor{
                            CursoId = _cursoId,
                            InstructorId = id
                        };
                        context.CursoInstructor.Add(cursoInstructor);
                    }
                }

                //Agregar precio al curso
                var precioEntidad = new Precio{
                    CursoId = _cursoId,
                    PrecioActual = request.Precio,
                    Promocion = request.Promocion,
                    PrecioId = Guid.NewGuid()
                };
                context.Precio.Add(precioEntidad);

                //Estado de la transaccion
                var valor =  await context.SaveChangesAsync();
                if(valor > 0){
                    return Unit.Value;
                }
                //Alerta de error
                throw new Exception("No se pudo insertar el curso");
            }
        }
    }
}