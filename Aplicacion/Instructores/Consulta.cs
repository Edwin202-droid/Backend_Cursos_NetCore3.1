using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Persistencia.DapperConexion.Instructor;

namespace Aplicacion.Instructores
{
    //Usamos instructorRepository de Persistencia para traer a los instructores
    public class Consulta
    {
        public class Lista : IRequest<List<InstructorModel>>{}

        public class Manejador : IRequestHandler<Lista, List<InstructorModel>>
        {
            private readonly IInstructor instructorRepository;

            public Manejador(IInstructor instructorRepository)
            {
                this.instructorRepository = instructorRepository;
            }
            public async Task<List<InstructorModel>> Handle(Lista request, CancellationToken cancellationToken)
            {   
                //retornar los valores en IEnumerable, lo queremos en List
                var resultado = await instructorRepository.ObtenerLista();

                return resultado.ToList();
            }
        }
    }
}