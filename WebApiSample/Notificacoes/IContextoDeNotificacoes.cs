using System.Collections.Generic;

namespace WebApiSample.Notificacoes
{
    public interface IContextoDeNotificacoes
    {
        IReadOnlyCollection<Notificacao> Notificacoes { get; }
        bool TemNotificacao { get; }

        void AdicionarNotificacao(string mensagem);
        void AdicionarNotificacao(Notificacao item);
    }
}
