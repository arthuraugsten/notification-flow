using MediatR;

namespace WebApiSample.Comandos
{
    public sealed class ExemploComando : IRequest<ExemploResultadoComando>
    {
        public string? Titulo { get; set; }
    }
}
