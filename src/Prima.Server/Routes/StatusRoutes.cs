using Prima.Core.Server.Data.Rest.Base;
using Prima.Core.Server.Extensions;

namespace Prima.Server.Routes;

public static class StatusRoutes
{
    public static IEndpointRouteBuilder MapStatusRoutes(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/status").WithTags("Status");

        group.MapGet(
                "/health",
                async () =>
                {
                    var statusRest = RestResultObject<string>.CreateSuccess();

                    statusRest.Data = "Server is running";

                    return statusRest.ToResult();
                }
            )
            .WithName("GetServerStatus")
            .WithDisplayName("Get Server Status");

        group.AllowAnonymous().ProducesValidationProblem();

        return endpoints;
    }
}
