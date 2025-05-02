using Prima.Core.Server.Data.Rest;
using Prima.Core.Server.Data.Rest.Base;
using Prima.Core.Server.Extensions;
using Prima.Core.Server.Interfaces.Services;

namespace Prima.Server.Routes;

public static class AuthRoutes
{
    public static IEndpointRouteBuilder MapAuthRoutes(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/auth").WithTags("Auth");


        group.MapPost(
            "/login",
            async (LoginRequestObject request, IAccountManager loginService) =>
            {
                var result = await loginService.LoginWebAsync(request);

                if (result.IsSuccess)
                {
                    return RestResultObject<LoginResponseObject>.CreateSuccess(result).ToResult();
                }

                return RestResultObject<LoginResponseObject>.CreateError(result.Message).ToResult();
            }
        );


        group.AllowAnonymous().ProducesValidationProblem();


        return endpoints;
    }
}
