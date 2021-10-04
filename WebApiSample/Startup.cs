using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Globalization;
using WebApiSample.Filtros;
using WebApiSample.Mediator;
using WebApiSample.Notificacoes;

namespace WebApiSample
{
    public sealed class Startup
    {
        private static readonly Type Type = typeof(Startup);

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection servicos)
        {
            servicos.AddMediatR(Type);

            AssemblyScanner
                .FindValidatorsInAssembly(Type.Assembly)
                .ForEach(result => servicos.AddScoped(result.InterfaceType, result.ValidatorType));

            ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("pt-BR");

            servicos.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidadorDeComportamento<,>));
            servicos.AddScoped<IContextoDeNotificacoes, ContextoDeNotificacoes>();

            servicos.AddControllers(opcoes => opcoes.Filters.Add<FiltroDeNotificacoes>());
            servicos.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApiSample", Version = "v1" }));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiSample v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
