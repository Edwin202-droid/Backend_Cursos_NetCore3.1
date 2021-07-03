using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Dominio;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Seguridad
{
    public class UsuarioActual
    {
        public class Ejecuta : IRequest<UsuarioData> { }

        public class Manejador : IRequestHandler<Ejecuta, UsuarioData>
        {
            private readonly UserManager<Usuario> userManager;
            private readonly IJwtGenerador jwtGenerador;
            private readonly IUsuarioSesion usuarioSesion;
            private readonly CursosOnlineContext context;

            public Manejador(UserManager<Usuario> userManager, IJwtGenerador jwtGenerador, IUsuarioSesion usuarioSesion, CursosOnlineContext context)
            {
                this.userManager = userManager;
                this.jwtGenerador = jwtGenerador;
                this.usuarioSesion = usuarioSesion;
                this.context = context;
            }
            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var usuario = await userManager.FindByNameAsync(usuarioSesion.ObtenerUsuarioSesion());

                //roles
                var resultadoRoles = await userManager.GetRolesAsync(usuario);
                //Conversion IList -> List
                var listaRoles = new List<string>(resultadoRoles);

                //Buscar imagen para mostrar
                var ImagenPerfil = await context.Documento.Where(x => x.ObjetoReferencia == new Guid(usuario.Id)).FirstOrDefaultAsync();
                if (ImagenPerfil != null)
                {
                    var imagenCliente = new ImagenGeneral
                    {
                        //Conversion byte a string
                        Data = Convert.ToBase64String(ImagenPerfil.Contenido),
                        Extension = ImagenPerfil.Extension,
                        Nombre = ImagenPerfil.Nombre
                    };
                    return new UsuarioData
                    {
                        NombreCompleto = usuario.NombreCompleto,
                        Username = usuario.UserName,
                        Token = jwtGenerador.CrearToken(usuario, listaRoles),
                        Email = usuario.Email,
                        
                        ImagenPerfil = imagenCliente
                    };
                }
                else
                {
                    return new UsuarioData
                    {
                        NombreCompleto = usuario.NombreCompleto,
                        Username = usuario.UserName,
                        Token = jwtGenerador.CrearToken(usuario, listaRoles),
                        Email = usuario.Email,
                        
                    };
                }

            }
        }
    }
}