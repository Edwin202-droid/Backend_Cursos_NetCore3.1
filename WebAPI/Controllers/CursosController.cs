using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aplicacion.Cursos;
using Dominio;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistencia.DapperConexion.Paginacion;

namespace WebAPI.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class CursosController : MiControllerBase
    {
        //Constructor Mediator viene de MiControllerBase

        [HttpGet]
        public async Task<ActionResult<List<CursoDTO>>> Get()
        {
            //El que se encarga de la logica, y traer los datos de la base de datos es Aplicacion
            return await Mediator.Send(new Consulta.ListaCursos());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CursoDTO>> Detalle(Guid id)
        {
            return await Mediator.Send(new ConsultaId.CursoUnico{Id = id});
        }
        //Nuevo.Ejecuta tiene lo que vamos a postear
        [HttpPost]
        public async Task<ActionResult<Unit>> Crear(Nuevo.Ejecuta data)
        {
            //Enviamos a Aplicacion
            return await Mediator.Send(data);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Unit>> Editar(Guid id, Editar.Ejecuta data)
        {
            data.CursoId = id;
            return await Mediator.Send(data);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> Eliminar(Guid id)
        {
            return await Mediator.Send(new Eliminar.Ejecuta{Id = id});
        }

        [HttpPost("report")]
        public async Task<ActionResult<PaginacionModel>> Report(PaginacionCurso.Ejecuta data)
        {
            return await Mediator.Send(data);
        }
    }
}