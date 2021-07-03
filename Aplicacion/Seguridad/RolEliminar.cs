using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aplicacion.Seguridad
{
    public class RolEliminar
    {
        public class Ejecuta : IRequest
        {
            public string Nombre { get; set; }
        }
        public class EjecutaValida : AbstractValidator<Ejecuta>
        {
            public EjecutaValida()
            {
                RuleFor( x => x.Nombre).NotEmpty();
            }
        }
        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly RoleManager<IdentityRole> roleManager;

            public Manejador(RoleManager<IdentityRole> roleManager)
            {
                this.roleManager = roleManager;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Rol exista, antes de evaluarlo
                var role = await roleManager.FindByNameAsync(request.Nombre);
                if(role == null)
                {
                    throw new ManejadorExcepcion(HttpStatusCode.BadRequest, new {mensaje = "No existe el rol"});
                }

                //Eliminar rol
                var resultado = await roleManager.DeleteAsync(role);
                if(resultado.Succeeded)
                {
                    return Unit.Value;
                }
                throw new Exception("No se pudo eliminar el rol");
            }
        }
    }
}