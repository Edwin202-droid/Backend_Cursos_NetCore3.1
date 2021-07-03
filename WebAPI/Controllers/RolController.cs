using System.Collections.Generic;
using System.Threading.Tasks;
using Aplicacion.Seguridad;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class RolController : MiControllerBase
    {
        [HttpPost("crear")]
        public async Task<ActionResult<Unit>> Crear(RolNuevo.Ejecuta parametros)
        {
            return await Mediator.Send(parametros);
        }

        [HttpDelete("eliminar")]
        public async Task<ActionResult<Unit>> Eliminar(RolEliminar.Ejecuta parametros)
        {
            return await Mediator.Send(parametros);
        }
        [HttpGet("lista")]
        public async Task<ActionResult<List<IdentityRole>>> Lista()
        {
            return await Mediator.Send(new RolLista.Ejecuta());
        }

        //Agregar rol a usuario 
        [HttpPost("agregarRolUsuario")]
        public async Task<ActionResult<Unit>> AgregarRolUsuario(UsuarioRolAgregar.Ejecuta parametros)
        {
            return await Mediator.Send(parametros);
        }
        //Eliminar rol a usuario
        [HttpPost("eliminarRolUsuario")]
        public async Task<ActionResult<Unit>> EliminarRolUsuario(UsuarioRolEliminar.Ejecuta parametros)
        {
            return await Mediator.Send(parametros);
        }

        //Listar los roles del usuario
        [HttpGet("{username}")]
        public async Task<ActionResult<List<string>>> ObtenerRolesUsuario(string username)
        {
            return await Mediator.Send(new ObtenerRolesUsuario.Ejecuta{Username = username});
        }
    }
}