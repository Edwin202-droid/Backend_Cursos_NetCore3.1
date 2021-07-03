using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dominio;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Documentos
{
    public class SubirArchivo
    {
        public class Ejecuta : IRequest
        {
            public string Data { get; set; }
            public string Nombre { get; set; }
            public string Extension { get; set; }
            public Guid? ObjetoReferencia { get; set; }//Para evaluar si existe
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
                /*======== Insertar archivo en la BD ============ */

                //Evaluar si el archivo existe
                var documento = await context.Documento.Where( x => x.ObjetoReferencia == request.ObjetoReferencia).FirstAsync();
                    //No hay archivos
                if(documento == null)
                {
                    var doc = new Documento{
                        Contenido = Convert.FromBase64String(request.Data),
                        Nombre = request.Nombre,
                        Extension = request.Extension,
                        DocumentoId = Guid.NewGuid(),
                        FechaCreacion = DateTime.UtcNow
                    };
                    context.Documento.Add(doc);
                }else
                {
                    documento.Contenido = Convert.FromBase64String(request.Data);
                    documento.Nombre = request.Nombre;
                    documento.Extension = request.Extension;
                    documento.FechaCreacion = DateTime.UtcNow;
                }
                var resultado = await context.SaveChangesAsync();
                if(resultado > 0 )
                {
                    return Unit.Value;
                }
                throw new Exception("No se pudo guardar el archivo");
            }
        }
    }
}