using Microsoft.AspNetCore.Mvc;
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
            )
            .Produces<RestResultObject<LoginResponseObject>>()
            .WithName("Login");


        group.MapPost(
                "/change_password",
                async ([FromBody] ChangePasswordObject changePassword, IAccountManager accountManager) =>
                {
                    var changed = await accountManager.ChangePasswordAsync(
                        changePassword.AccountName,
                        changePassword.OldPassword,
                        changePassword.NewPassword
                    );

                    return RestResultObject<bool>.CreateSuccess(changed).ToResult();
                }
            )
            .WithName("ChangePassword")
            .Produces<RestResultObject<bool>>();


        group.AllowAnonymous().ProducesValidationProblem();


        return endpoints;
    }
}
