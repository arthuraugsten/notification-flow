using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebApiSample.Filtros;
using WebApiSample.Notificacoes;
using Xunit;

namespace WebApiSampleTests.Filtros
{
    public sealed class FiltroDeNotificacoesTests
    {
        private readonly FiltroDeNotificacoes _filtrosDeNotificacao;
        private readonly ResultExecutingContext _contextoExecucao;

        private readonly Mock<IContextoDeNotificacoes> _contextoDeNotificacao = new();
        private readonly Mock<ResultExecutionDelegate> _delegarExecucao = new();

        public FiltroDeNotificacoesTests()
        {
            _contextoExecucao = new ResultExecutingContext(
                new ActionContext(
                    httpContext: new DefaultHttpContext(),
                    routeData: new RouteData(),
                    actionDescriptor: new ActionDescriptor(),
                    modelState: new ModelStateDictionary()
                ),
                Array.Empty<IFilterMetadata>(),
                Mock.Of<IActionResult>(),
                Mock.Of<Controller>()
            );

            _contextoExecucao.HttpContext.Response.ContentType = MediaTypeNames.Application.Json;
            _filtrosDeNotificacao = new FiltroDeNotificacoes(_contextoDeNotificacao.Object);
        }

        [Fact]
        public async Task Deve_gerar_erro_ao_executar_um_contexto_nulo()
        {
            var excecao = await Assert.ThrowsAsync<ArgumentNullException>(async ()
                => await _filtrosDeNotificacao.OnResultExecutionAsync(null, _delegarExecucao.Object));

            excecao.Message.Should().Be("Value cannot be null. (Parameter 'contexto')");
            _delegarExecucao.Verify(t => t.Invoke(), Times.Never);
            _contextoDeNotificacao.Verify(t => t.TemNotificacao, Times.Never);
            _contextoDeNotificacao.Verify(t => t.Notificacoes, Times.Never);
        }

        [Fact]
        public async Task Deve_gerar_erro_quando_a_execucao_do_delegado_for_nula()
        {
            var excecao = await Assert.ThrowsAsync<ArgumentNullException>(async ()
                => await _filtrosDeNotificacao.OnResultExecutionAsync(_contextoExecucao, null));

            excecao.Message.Should().Be("Value cannot be null. (Parameter 'proximo')");

            _delegarExecucao.Verify(t => t.Invoke(), Times.Never);
            _contextoDeNotificacao.Verify(t => t.TemNotificacao, Times.Never);
            _contextoDeNotificacao.Verify(t => t.Notificacoes, Times.Never);
        }

        [Fact]
        public async Task Deve_executar_o_delegado_quando_nao_houver_notificacoes()
        {
            var resultadoEsperado = new byte[] { 1, 2, 3, 4, 5 };

            _contextoDeNotificacao.Setup(t => t.TemNotificacao).Returns(false);
            _contextoExecucao.HttpContext.Response.Body = new MemoryStream(resultadoEsperado);

            await _filtrosDeNotificacao.OnResultExecutionAsync(_contextoExecucao, _delegarExecucao.Object);

            _contextoExecucao.HttpContext.Response.Body.Position = 0;

            var texto = await new StreamReader(_contextoExecucao.HttpContext.Response.Body).ReadToEndAsync();

            texto.Should().Be(Encoding.UTF8.GetString(resultadoEsperado));
            _contextoExecucao.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
            _contextoExecucao.HttpContext.Response.ContentType.Should().Be(MediaTypeNames.Application.Json);
            _delegarExecucao.Verify(t => t.Invoke(), Times.Once);
            _contextoDeNotificacao.Verify(t => t.TemNotificacao, Times.Once);
            _contextoDeNotificacao.Verify(t => t.Notificacoes, Times.Never);
        }

        [Fact]
        public async Task Deve_escrever_erros_ao_validar_o_contexto()
        {
            var notificacoes = new[] { new Notificacao("Teste!") };

            _contextoDeNotificacao.Setup(t => t.TemNotificacao).Returns(true);
            _contextoDeNotificacao.Setup(t => t.Notificacoes).Returns(notificacoes);
            _contextoExecucao.HttpContext.Response.Body = new MemoryStream();

            await _filtrosDeNotificacao.OnResultExecutionAsync(_contextoExecucao, _delegarExecucao.Object);

            _contextoExecucao.HttpContext.Response.Body.Position = 0;

            var texto = await new StreamReader(_contextoExecucao.HttpContext.Response.Body).ReadToEndAsync();
            var resultado = JsonSerializer.Deserialize<ApiReturnItem<object>>(texto);

            resultado.Should().BeEquivalentTo(new
            {
                Item = default(object),
                Sucesso = false,
                Mensagem = new ApiReturnMessage
                {
                    Title = "Erro ao executar a requisição!",
                    Details = notificacoes.Select(t => t.Mensagem).ToList()
                }
            });

            _contextoExecucao.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            _contextoExecucao.HttpContext.Response.ContentType.Should().Be(MediaTypeNames.Application.Json);
            _delegarExecucao.Verify(t => t.Invoke(), Times.Never);
            _contextoDeNotificacao.Verify(t => t.Notificacoes, Times.Once);
            _contextoDeNotificacao.Verify(t => t.TemNotificacao, Times.Once);
        }
    }
}
