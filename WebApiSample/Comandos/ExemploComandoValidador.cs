using FluentValidation;

namespace WebApiSample.Comandos
{
    public class ExemploComandoValidador : AbstractValidator<ExemploComando>
    {
        public ExemploComandoValidador()
            => RuleFor(t => t.Titulo).NotEmpty();
    }
}
