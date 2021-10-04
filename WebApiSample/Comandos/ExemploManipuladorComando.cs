using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiSample.Comandos
{
    public class ExemploManipuladorComando : IRequestHandler<ExemploComando, ExemploResultadoComando>
    {
        public Task<ExemploResultadoComando> Handle(ExemploComando request, CancellationToken cancellationToken)
            => Task.FromResult(new ExemploResultadoComando { Mensagem = "Sucesso!" });
    }
}
