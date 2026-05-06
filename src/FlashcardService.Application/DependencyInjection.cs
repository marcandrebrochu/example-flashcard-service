using System.Reflection;
using FlashcardService.Application.Common.Behaviors;
using FlashcardService.Application.Common.Interfaces;
using FlashcardService.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FlashcardService.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            config.AddOpenRequestPreProcessor(typeof(LoggingBehavior<>));
            config.AddOpenBehavior(typeof(PerformanceBehavior<,>));
        });

        builder.Services.AddSingleton<IIdentifierService, IdentifierService>();
    }
}