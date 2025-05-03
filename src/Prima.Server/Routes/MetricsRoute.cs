using Orion.Core.Server.Data.Metrics.Diagnostic;
using Orion.Core.Server.Interfaces.Services.System;
using Prima.Core.Server.Data.Rest.Base;
using Prima.Core.Server.Extensions;

namespace Prima.Server.Routes;

public static class MetricsRoute
{
    public static IEndpointRouteBuilder MapMetricsRoutes(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/metrics").WithTags("Metrics");


        group.MapGet(
                "/",
                async (IDiagnosticService diagnosticService) =>
                {
                    var metricsRest = RestResultObject<Dictionary<string, object>>.CreateSuccess();

                    metricsRest.Data = diagnosticService.GetAllProvidersMetrics();

                    return metricsRest.ToResult();
                }
            )
            .Produces<RestResultObject<Dictionary<string, object>>>()
            .WithDescription("Get Metrics")
            .WithName("GetMetrics");


        return group;
    }
}
