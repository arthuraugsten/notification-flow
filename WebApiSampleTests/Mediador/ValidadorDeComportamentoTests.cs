using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiSample.Mediator;
using WebApiSample.Notificacoes;
using Xunit;

namespace WebApiSampleTests.Mediador
{
    public sealed class ValidadorDeComportamentoTests
    {
        private ValidadorDeComportamento<CommandMock, CommandResultMock> _validadorComportamento;
        private IEnumerable<IValidator<CommandMock>> _validadores;

        private readonly Mock<IContextoDeNotificacoes> _contextoDeNotificacoes = new();
        private readonly Mock<RequestHandlerDelegate<CommandResultMock>> _delegado = new();
        private readonly CommandMock _comando = new() { Name = "Teste" };

        public static List<object[]> CenarioValidadores { get; } = new(2)
        {
            new object[] { Enumerable.Empty<IValidator<CommandMock>>() },
            new object[] { default(List<IValidator<CommandMock>>) }
        };

        [Fact]
        public async Task Deve_retornar_excecao_quando_a_proximo_delegacao_for_nulo()
        {
            _validadores = Enumerable.Empty<IValidator<CommandMock>>();
            _validadorComportamento = new ValidadorDeComportamento<CommandMock, CommandResultMock>(_validadores, _contextoDeNotificacoes.Object);

            var excecao = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _validadorComportamento.Handle(_comando, default, default));

            excecao.Message.Should().Be("Value cannot be null. (Parameter 'proximo')");
            _contextoDeNotificacoes.VerifyNoOtherCalls();
            _delegado.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(CenarioValidadores))]
        public async Task Deve_retornar_sucesso_quando_nao_houver_validadores(IEnumerable<IValidator<CommandMock>> validadores)
        {
            var resultadoEsperado = new CommandResultMock();

            _validadores = Enumerable.Empty<IValidator<CommandMock>>();
            _delegado.Setup(t => t.Invoke()).ReturnsAsync(resultadoEsperado);
            _validadorComportamento = new ValidadorDeComportamento<CommandMock, CommandResultMock>(validadores, _contextoDeNotificacoes.Object);

            var resultado = await _validadorComportamento.Handle(_comando, default, _delegado.Object);

            _contextoDeNotificacoes.VerifyNoOtherCalls();
            _delegado.Verify(t => t.Invoke(), Times.Once);

            resultado.Should().BeSameAs(resultadoEsperado);
        }

        [Fact]
        public async Task Deve_retornar_nulo_quando_a_validacao_falhar()
        {
            _comando.Name = string.Empty;
            _validadores = new[] { new CommandMockValidation() };

            _validadorComportamento = new ValidadorDeComportamento<CommandMock, CommandResultMock>(_validadores, _contextoDeNotificacoes.Object);

            var resultado = await _validadorComportamento.Handle(_comando, default, _delegado.Object);

            _contextoDeNotificacoes.Verify(t => t.AdicionarNotificacao(CommandMockValidation.Mensagem), Times.Once);
            _delegado.VerifyNoOtherCalls();

            resultado.Should().BeNull();
        }

        [Fact]
        public async Task Deve_retornar_sucesso_quando_a_validacao_tiver_sucesso()
        {
            var resultadoEsperado = new CommandResultMock();

            _validadores = new[] { new CommandMockValidation() };
            _delegado.Setup(t => t.Invoke()).ReturnsAsync(resultadoEsperado);
            _validadorComportamento = new ValidadorDeComportamento<CommandMock, CommandResultMock>(_validadores, _contextoDeNotificacoes.Object);

            var resultado = await _validadorComportamento.Handle(_comando, default, _delegado.Object);

            _contextoDeNotificacoes.VerifyNoOtherCalls();
            _delegado.Verify(t => t.Invoke(), Times.Once);
            resultado.Should().BeSameAs(resultadoEsperado);
        }

        public sealed class CommandMock : IRequest<CommandResultMock>
        {
            public string Name { get; set; }
        }

        public sealed class CommandResultMock { }

        public sealed class CommandMockValidation : AbstractValidator<CommandMock>
        {
            public static string Mensagem = "Message";

            public CommandMockValidation()
                => RuleFor(t => t.Name).NotEmpty().WithMessage(Mensagem);
        }
    }
}
