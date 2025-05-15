using Microsoft.Extensions.DependencyInjection;
using Orion.Core.Server.Extensions;
using Prima.Core.Server.Interfaces.Services;
using Prima.JavaScript.Engine.Data.Configs;
using Prima.JavaScript.Engine.Services;

namespace Prima.JavaScript.Engine.Extensions;

public static class ServicesCollection
{
    public static IServiceCollection AddJsScriptEngineService(
        this IServiceCollection services, ScriptEngineConfig? config = null
    )
    {
        config ??= new ScriptEngineConfig();
        services.AddSingleton(config);
        return services.AddService<IScriptEngineService, ScriptEngineService>();
    }
}
