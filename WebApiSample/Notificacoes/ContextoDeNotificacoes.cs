using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace WebApiSample.Notificacoes
{
    public sealed class ContextoDeNotificacoes : IContextoDeNotificacoes
    {
        private readonly List<Notificacao> _notificacoes = new();

        public IReadOnlyCollection<Notificacao> Notificacoes => _notificacoes.AsReadOnly();
        public bool TemNotificacao => _notificacoes.Any();

        public void AdicionarNotificacao([AllowNull] string mensagem)
        {
            if (string.IsNullOrWhiteSpace(mensagem))
                return;

            _notificacoes.Add(new(mensagem));
        }

        public void AdicionarNotificacao([AllowNull] Notificacao item)
        {
            if (item is null)
                return;

            _notificacoes.Add(item);
        }
    }
}
