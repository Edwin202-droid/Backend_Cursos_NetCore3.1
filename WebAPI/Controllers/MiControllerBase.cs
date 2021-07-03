using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace WebAPI.Controllers
{   
    //Creamos un controller para mediator, asi no lo inyectamos en todos los controllers
    [Route("api/[controller]")]
    [ApiController]
    public class MiControllerBase : ControllerBase  
    {
        private IMediator _mediator;

        protected IMediator Mediator => _mediator ?? (_mediator = HttpContext.RequestServices.GetService<IMediator>());
    }
}