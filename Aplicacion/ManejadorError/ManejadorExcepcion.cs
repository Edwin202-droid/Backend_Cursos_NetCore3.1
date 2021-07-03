using System;
using System.Net;

namespace Aplicacion.ManejadorError
{
    public class ManejadorExcepcion : Exception
    {
        public HttpStatusCode codigo {get; }
        public object errores {get; }

        public ManejadorExcepcion(HttpStatusCode codigo, object errores = null)
        {
            this.codigo = codigo;
            this.errores = errores;
        }
    }
}