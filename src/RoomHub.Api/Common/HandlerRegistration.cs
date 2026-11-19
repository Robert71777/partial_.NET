using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace RoomHub.Api.Common;

public static class HandlerRegistration
{
    /// <summary>
    /// Scans <paramref name="assembly"/> and registers every concrete
    /// ICommandHandler&lt;,&gt; and IQueryHandler&lt;,&gt; implementation as scoped.
    /// </summary>
    public static IServiceCollection AddHandlers(this IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false });

        foreach (var type in types)
        {
            foreach (var iface in type.GetInterfaces())
            {
                if (!iface.IsGenericType) continue;

                var definition = iface.GetGenericTypeDefinition();
                if (definition == typeof(ICommandHandler<,>) ||
                    definition == typeof(IQueryHandler<,>))
                {
                    services.AddScoped(iface, type);
                }
            }
        }

        return services;
    }
}
