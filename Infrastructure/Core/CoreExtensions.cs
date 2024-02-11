using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Core.Commands;
using Infrastructure.Core.Events;
using Infrastructure.Core.Queries;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Infrastructure.Core
{
    public static class CoreExtensions
    {
        public static IServiceCollection AddCore(this IServiceCollection services, params Type[] types)
        {
            var assemblies = types.Select(type => type.GetTypeInfo().Assembly);

            services.AddMediatR(x => x.RegisterServicesFromAssemblies(assemblies.ToArray()));

            services.AddScoped<ICommandBus, CommandBus>();
            services.AddScoped<IQueryBus, QueryBus>();
            services.AddScoped<IEventBus, EventBus>();

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddOptions();

            services.AddMvc(opt => { opt.Filters.Add<ExceptionFilter>(); });

            services.AddHealthChecks();

            services.AddControllers()
                .AddNewtonsoftJson();

            services.AddValidatorsFromAssemblies(assemblies);
            services.AddFluentValidationAutoValidation(); // the same old MVC pipeline behavior
            services.AddFluentValidationClientsideAdapters(); // for client side


            return services;
        }

        public static IApplicationBuilder UseCore(this IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            return app;
        }
    }
}
