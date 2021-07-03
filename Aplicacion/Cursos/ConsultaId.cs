using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using AutoMapper;
using Dominio;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class ConsultaId
    {
        public class CursoUnico : IRequest<CursoDTO>{
            public Guid Id {get; set;}
        }

        public class Manejador : IRequestHandler<CursoUnico, CursoDTO>
        {
            private readonly CursosOnlineContext context;
            private readonly IMapper mapper;

            public Manejador(CursosOnlineContext context, IMapper mapper)
            {
                this.context = context;
                this.mapper = mapper;
            }

            public async Task<CursoDTO> Handle(CursoUnico request, CancellationToken cancellationToken)
            {
                //Traer al instructor
                var curso = await  context.Curso.Include(x => x.ComentarioLista)
                                                .Include(x => x.PrecioPromocion)
                                                .Include(x => x.InstructoresLink)
                                                .ThenInclude(y => y.Instructor)
                                                .FirstOrDefaultAsync( a => a.CursoId == request.Id);
                if(curso == null)
                {
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new {mensaje= "No se encontro el curso"});
                }
                //Mapear
                var cursoDTO = mapper.Map<Curso, CursoDTO>(curso);

                return cursoDTO;
            }
        }

    }
}