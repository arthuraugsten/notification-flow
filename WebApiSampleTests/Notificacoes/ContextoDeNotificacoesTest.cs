using FluentAssertions;
using WebApiSample.Notificacoes;
using Xunit;

namespace WebApiSampleTests.Notificacoes
{
    public sealed class ContextoDeNotificacoesTest
    {
        private readonly ContextoDeNotificacoes _contexto = new();

        [Fact]
        public void Deve_inicializar_lista_interna_de_notificacoes()
        {
            _contexto.Notificacoes.Should().BeEmpty();
            _contexto.TemNotificacao.Should().BeFalse();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Deve_ignorar_strings_invalidas_na_adicao_de_mensagens(string input)
        {
            _contexto.AdicionarNotificacao(input);
            _contexto.Notificacoes.Should().BeEmpty();
            _contexto.TemNotificacao.Should().BeFalse();
        }

        [Fact]
        public void Deve_adicionar_mensagem_por_string()
        {
            const string input = "abc";

            _contexto.AdicionarNotificacao(input);
            _contexto.Notificacoes.Should().BeEquivalentTo(new[] { new Notificacao(input) });
            _contexto.TemNotificacao.Should().BeTrue();
        }

        [Fact]
        public void Deve_ignorar_notificacao_nula_ao_adicionar()
        {
            _contexto.AdicionarNotificacao(default(Notificacao));
            _contexto.Notificacoes.Should().BeEmpty();
            _contexto.TemNotificacao.Should().BeFalse();
        }

        [Fact]
        public void Deve_adicionar_notificacao()
        {
            var notificacao = new Notificacao("abc");

            _contexto.AdicionarNotificacao(notificacao);
            _contexto.Notificacoes.Should().BeEquivalentTo(new[] { notificacao });
            _contexto.TemNotificacao.Should().BeTrue();
        }
    }
}
