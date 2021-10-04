using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApiSample.Comandos;

namespace WebApiSample.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public sealed class SampleController : ControllerBase
    {
        private readonly IMediator _mediador;

        public SampleController(IMediator mediador)
            => _mediador = mediador;

        [HttpPost]
        public async Task<IActionResult> GetAsync([FromBody] ExemploComando comando)
        {
            var resultado = await _mediador.Send(comando);
            return Ok(resultado);
        }
    }
}
