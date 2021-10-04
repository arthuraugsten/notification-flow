namespace WebApiSample.Filtros
{
    public sealed class ApiReturnItem<T> where T : new()
    {
        public T? Item { get; set; }
        public ApiReturnMessage? Mensagem { get; set; }
        public bool Sucesso { get; set; }
        public string? Versao { get; set; }
    }
}
