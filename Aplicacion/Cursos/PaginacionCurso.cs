using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Persistencia.DapperConexion.Paginacion;

namespace Aplicacion.Cursos
{
    public class PaginacionCurso
    {
        public class Ejecuta : IRequest<PaginacionModel>
        {
            public string Titulo { get; set; }
            public int NumeroPagina { get; set; }
            public int CantidadElementos { get; set; }

        }
        public class Manejador : IRequestHandler<Ejecuta, PaginacionModel>
        {
            private readonly IPaginacion paginacion;

            public Manejador(IPaginacion paginacion)
            {
                this.paginacion = paginacion;
            }
            public async Task<PaginacionModel> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var storeProcedure = "usp_Obtener_Cursos_Paginacion";
                var ordenamiento = "Titulo";
                var parametros = new Dictionary<string, object>();
                parametros.Add("NombreCurso",request.Titulo);
                return await paginacion.devolverPaginacion(storeProcedure,request.NumeroPagina, request.CantidadElementos,parametros,ordenamiento);
            }
        }
    }
}