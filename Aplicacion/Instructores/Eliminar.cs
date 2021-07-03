using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Persistencia.DapperConexion.Instructor;

namespace Aplicacion.Instructores
{
    public class Eliminar
    {
        public class Ejecuta : IRequest
        {
            public Guid Id { get; set; }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly IInstructor instructorRepositorio;

            public Manejador(IInstructor instructorRepositorio)
            {
                this.instructorRepositorio = instructorRepositorio;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var resultados = await instructorRepositorio.Eliminar(request.Id);
                if(resultados > 0)
                {
                    return Unit.Value;
                }
                throw new Exception("No se pudo eliminar el instructor");
            }
        }
    }
}