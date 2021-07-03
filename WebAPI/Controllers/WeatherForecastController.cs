using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Persistencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly CursosOnlineContext context;

        public WeatherForecastController(CursosOnlineContext _context)
        {
            this.context = _context;
        }
        [HttpGet]
        public IEnumerable<string> Get(){
            string[] nombres = new[]{"Manchas","Luis Miguel"};
            return nombres;
        }
    }
}
