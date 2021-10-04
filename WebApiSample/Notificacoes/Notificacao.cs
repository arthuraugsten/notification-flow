using System.Diagnostics.CodeAnalysis;

namespace WebApiSample.Notificacoes
{
    public sealed record Notificacao
    {
        public Notificacao([DisallowNull] string mensagem)
            => Mensagem = mensagem;

        public string Mensagem { get; }
    }
}
