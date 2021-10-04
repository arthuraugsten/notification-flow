using FluentAssertions;
using WebApiSample.Notificacoes;
using Xunit;

namespace WebApiSampleTests.Notificacoes
{
    public sealed class NotificacaoTests
    {
        [Fact]
        public void Deve_construir_corretamente()
        {
            const string texto = "abc";

            var notificacao = new Notificacao(texto);
            notificacao.Mensagem.Should().Be(texto);
        }
    }
}
