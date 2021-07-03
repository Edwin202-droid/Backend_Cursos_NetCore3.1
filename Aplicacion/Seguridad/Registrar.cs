using System.Net;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Aplicacion.ManejadorError;
using Dominio;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistencia;
using System;
using FluentValidation;

namespace Aplicacion.Seguridad
{
    public class Registrar
    {
        //IRequest: lo que devolvera
        public class Ejecuta : IRequest<UsuarioData>
        {
            //Lo que pide
            public string Nombre { get; set; }
            public string Apellidos { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string UserName { get; set; }
        }

        //Validacion
        public class EjecutaValidador : AbstractValidator<Ejecuta>
        {
            public EjecutaValidador()
            {
                RuleFor(x => x.Nombre).NotEmpty();
                RuleFor(x => x.Apellidos).NotEmpty();
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
                RuleFor(x => x.UserName).NotEmpty();
            }
        }

        //ejecuta: lo que implementara, usuariodata: lo que devolvera
        public class Manejador : IRequestHandler<Ejecuta, UsuarioData>
        {
            private readonly CursosOnlineContext context;
            private readonly UserManager<Usuario> userManager;
            private readonly IJwtGenerador jwtGenerador;

            //Lo que usaremos para implementar la funcion
            public Manejador(CursosOnlineContext context, UserManager<Usuario> userManager, IJwtGenerador jwtGenerador)
            {
                this.context = context;
                this.userManager = userManager;
                this.jwtGenerador = jwtGenerador;
            }
            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Verificar que el email a registrar no exista, anyasync= boolean
                var existe = await context.Users.Where( x => x.Email == request.Email).AnyAsync();
                if(existe)
                {
                    throw new ManejadorExcepcion(HttpStatusCode.BadRequest, new {mensaje = "El email ya existe"});
                }
                //Verificar que el username no exista
                var existeUsername = await context.Users.Where(x => x.UserName == request.UserName).AnyAsync();
                if(existeUsername)
                {
                    throw new ManejadorExcepcion(HttpStatusCode.BadRequest, new {mensaje = "El Username existe ya existe"});
                }

                //Continuamos con el email y usuario verificado
                var usuario = new Usuario{
                    NombreCompleto = request.Nombre + " " + request.Apellidos,
                    Email = request.Email,
                    UserName = request.UserName
                };
                var resultado = await userManager.CreateAsync(usuario, request.Password);
                if(resultado.Succeeded)
                {
                    return new UsuarioData{
                        NombreCompleto = usuario.NombreCompleto,
                        //roles = null, porque recien estamos creando al usuario
                        Token = jwtGenerador.CrearToken(usuario, null),
                        Username= usuario.UserName
                    };
                }
                throw new Exception("No se pudo agregar al nuevo usuario");
            }
        }
    }
}