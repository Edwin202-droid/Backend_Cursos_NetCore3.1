using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Intrinsics.X86;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Seguridad
{
    public class UsuarioActualizar
    {
        public class Ejecuta : IRequest<UsuarioData>
        {
            public string Nombre { get; set; }
            public string Apellidos { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Username { get; set; }

            //Agregar foto, Clase ImagenGeneral.cs
            public ImagenGeneral ImagenPerfil { get; set; }
        }

        public class EjecutaValidador : AbstractValidator<Ejecuta>
        {
            public EjecutaValidador()
            {
                RuleFor(x => x.Nombre).NotEmpty();
                RuleFor(x => x.Apellidos).NotEmpty();
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
                RuleFor(x => x.Username).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta, UsuarioData>
        {
            private readonly CursosOnlineContext context;
            private readonly UserManager<Usuario> userManager;
            private readonly IJwtGenerador jwtGenerador;

            //Para poder actualizar la contraseña del usuario
            private readonly IPasswordHasher<Usuario> passwordHasher;

            public Manejador(CursosOnlineContext context, UserManager<Usuario> userManager, IJwtGenerador jwtGenerador, IPasswordHasher<Usuario> passwordHasher)
            {
                this.context = context;
                this.userManager = userManager;
                this.jwtGenerador = jwtGenerador;
                this.passwordHasher = passwordHasher;
            }
            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Evaluar que el usuario exista
                var usuarioIden = await userManager.FindByNameAsync(request.Username);
                if (usuarioIden == null)
                {
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new { mensaje = "No existe este Username" });
                }
                //Ok. Existe
                //Evaluar si existe algun usuario con el email que introducimos
                var resultado = await context.Users.Where(x => x.Email == request.Email && x.UserName != request.Username).AnyAsync();
                if (resultado)
                {
                    //Si es verdad
                    throw new ManejadorExcepcion(HttpStatusCode.InternalServerError, new { mensaje = "Este email pertenece a otro usuario" });
                }

                //Imagen -> si no mando una imagen, ignoro
                //Opcional
                if (request.ImagenPerfil != null)
                {
                    //Buscar una imagen en la tabla documentos por el id del usuario
                    var resultadoImagen = await context.Documento.Where(x => x.ObjetoReferencia == new Guid(usuarioIden.Id)).FirstAsync();
                    //Si el usuario no tiene imagen
                    if (resultadoImagen == null)
                    {
                        var imagen = new Documento
                        {
                            Contenido = System.Convert.FromBase64String(request.ImagenPerfil.Data),
                            Nombre = request.ImagenPerfil.Nombre,
                            Extension = request.ImagenPerfil.Extension,
                            //Saber aquien le va a pertenecer. Guid
                            ObjetoReferencia = new Guid(usuarioIden.Id),
                            DocumentoId = Guid.NewGuid(),
                            FechaCreacion = DateTime.UtcNow
                        };
                        //Agregar imagen a BD
                        context.Documento.Add(imagen);
                    }
                    //Si el usuario tiene una imagen
                    resultadoImagen.Contenido = System.Convert.FromBase64String(request.ImagenPerfil.Data);
                    resultadoImagen.Nombre = request.ImagenPerfil.Nombre;
                    resultadoImagen.Extension = request.ImagenPerfil.Extension;
                }


                //Ok, usuario existe y el email es unico
                usuarioIden.NombreCompleto = request.Nombre + " " + request.Apellidos;
                usuarioIden.PasswordHash = passwordHasher.HashPassword(usuarioIden, request.Password);//Actualizar contraseña
                usuarioIden.Email = request.Email;

                var resultadoUpdate = await userManager.UpdateAsync(usuarioIden);
                //Obtener roles para el token
                var resultadoRoles = await userManager.GetRolesAsync(usuarioIden);
                //Conversion
                var listRoles = new List<string>(resultadoRoles);

                //Imagen
                var imagenPerfil = await context.Documento.Where( x => x.ObjetoReferencia == new Guid(usuarioIden.Id)).FirstOrDefaultAsync();
                ImagenGeneral imagenGeneral = null;
                if(imagenGeneral != null)
                {
                    imagenGeneral = new ImagenGeneral{
                        Data = Convert.ToBase64String(imagenPerfil.Contenido),
                        Nombre= imagenPerfil.Nombre,
                        Extension= imagenPerfil.Extension
                    };
                }
                if (resultadoUpdate.Succeeded)
                {
                    return new UsuarioData
                    {
                        NombreCompleto = usuarioIden.NombreCompleto,
                        Username = usuarioIden.UserName,
                        Email = usuarioIden.Email,
                        Token = jwtGenerador.CrearToken(usuarioIden, listRoles),
                        ImagenPerfil= imagenGeneral
                    };
                }

                throw new Exception("No se pudo actualizar el usuario");

            }
        }
    }
}