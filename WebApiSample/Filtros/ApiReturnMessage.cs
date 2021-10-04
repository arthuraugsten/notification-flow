using System.Collections.Generic;

namespace WebApiSample.Filtros
{
    public sealed class ApiReturnMessage
    {
        public string? Title { get; set; }
        public IEnumerable<string>? Details { get; set; }
    }
}
