using Prima.Core.Server.Interfaces.Services;

namespace Prima.Server.Routes;

public static class AccountRoutes
{
    public static IEndpointRouteBuilder MapAccountRoutes(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/account").WithTags("Account");


        group.MapPost(
                "/register",
                (IAccountManager accountManager) => { }
            )
            .WithName("RegisterAccount")
            .WithDisplayName("Register Account");


        return endpoints;
    }
}
