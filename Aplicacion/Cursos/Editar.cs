using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Editar
    {
        //Lo que ocuparemos
        public class Ejecuta : IRequest
        {
            public Guid CursoId { get; set; }
            public string Titulo { get; set; }
            public string Descripcion { get; set; }
            public DateTime? FechaPublicacion { get; set; }

            //Instructores
            public List<Guid> ListaInstructor { get; set; }
            public decimal? Precio { get; set; }
            public decimal? Promocion { get; set; }
        }

        //Validaciones con Fluent
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
                var curso = await context.Curso.FindAsync(request.CursoId);
                if (curso == null)
                {
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new {mensaje= "No se encontro el curso"});
                }

                //Actualizar datos, si no actualiza un campo, conservar
                curso.Titulo = request.Titulo ?? curso.Titulo;
                curso.Descripcion= request.Descripcion ?? curso.Descripcion;
                curso.FechaPublicacion= request.FechaPublicacion ?? curso.FechaPublicacion;
                curso.FechaCreacion=DateTime.UtcNow;
                
                //Actualizar precio del curso
                var precioEntidad = context.Precio.Where( x => x.CursoId == curso.CursoId).FirstOrDefault();
                //Si ya tenemos un precio, actualizar
                if(precioEntidad != null)
                {
                    precioEntidad.Promocion = request.Promocion ?? precioEntidad.Promocion;
                    precioEntidad.PrecioActual = request.Precio ?? precioEntidad.PrecioActual;
                }else{
                    //Si no tenemos precio, agregar
                    precioEntidad= new Precio{
                        PrecioId = Guid.NewGuid(),
                        PrecioActual = request.Precio ?? 0,
                        Promocion= request.Promocion ?? 0,
                        CursoId= curso.CursoId
                    };
                    await context.Precio.AddAsync(precioEntidad);
                }

                //Instructores
                if(request.ListaInstructor != null)
                {
                    if(request.ListaInstructor.Count > 0 )
                    {
                        //Eliminar los instructores actuales del curso en la BD
                        var instructoresDB = context.CursoInstructor.Where(x => x.CursoId == request.CursoId).ToList();
                        foreach (var instructorEliminar in instructoresDB)
                        {
                            context.CursoInstructor.Remove(instructorEliminar);
                        }
                        //Se elimino los instructores

                        //Agregar instructor del cliente
                        foreach (var id in request.ListaInstructor)
                        {
                            var nuevoInstructor = new CursoInstructor{
                                CursoId = request.CursoId,
                                InstructorId = id
                            };
                            context.CursoInstructor.Add(nuevoInstructor);
                        }

                    }
                }

                var resultado = await context.SaveChangesAsync();
                if(resultado > 0)
                {
                    return Unit.Value;
                }

                throw new Exception("No se guardaron los cambios en el curso");
            }
        }
    }
}