using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using WebApiSample.Notificacoes;

namespace WebApiSample.Filtros
{
    public sealed class FiltroDeNotificacoes : IAsyncResultFilter
    {
        private readonly IContextoDeNotificacoes _contextoDeNotificacao;

        public FiltroDeNotificacoes(IContextoDeNotificacoes contextoDeNotificacao)
            => _contextoDeNotificacao = contextoDeNotificacao;

        public async Task OnResultExecutionAsync(ResultExecutingContext contexto, ResultExecutionDelegate proximo)
        {
            _ = contexto ?? throw new ArgumentNullException(nameof(contexto));
            _ = proximo ?? throw new ArgumentNullException(nameof(proximo));

            if (_contextoDeNotificacao.TemNotificacao)
            {
                contexto.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                contexto.HttpContext.Response.ContentType = MediaTypeNames.Application.Json;

                var retorno = new ApiReturnItem<object>
                {
                    Item = null,
                    Sucesso = false,
                    Mensagem = new ApiReturnMessage
                    {
                        Title = "Erro ao executar a requisição!",
                        Details = _contextoDeNotificacao.Notificacoes.Select(t => t.Mensagem)
                    }
                };

                var objetoSerializado = JsonSerializer.Serialize(retorno);

                contexto.HttpContext.Response.Body.Position = 0;
                await contexto.HttpContext.Response.WriteAsync(objetoSerializado);

                return;
            }

            await proximo();
        }
    }
}
