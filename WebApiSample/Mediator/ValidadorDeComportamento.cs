using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApiSample.Notificacoes;

namespace WebApiSample.Mediator
{
    public sealed class ValidadorDeComportamento<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse?> where TRequest : IRequest<TResponse?>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validadores;
        private readonly IContextoDeNotificacoes _contextoDeNotificacoes;

        public ValidadorDeComportamento(IEnumerable<IValidator<TRequest>> validadores, IContextoDeNotificacoes contextoDeNotificacoes)
        {
            _validadores = validadores;
            _contextoDeNotificacoes = contextoDeNotificacoes;
        }

        public async Task<TResponse?> Handle(TRequest requisicao, CancellationToken tokenDeCancelamento, RequestHandlerDelegate<TResponse?> proximo)
        {
            _ = proximo ?? throw new ArgumentNullException(nameof(proximo));
            var contexto = new ValidationContext<TRequest>(requisicao);

            foreach (var validador in _validadores ?? Array.Empty<IValidator<TRequest>>())
            {
                if (await validador.ValidateAsync(contexto) is { IsValid: false } resultadoValidacao)
                {
                    foreach (var erro in resultadoValidacao.Errors.Where(t => t is not null))
                        _contextoDeNotificacoes.AdicionarNotificacao(erro.ErrorMessage);

                    return default;
                }
            }

            return await proximo();
        }
    }
}
