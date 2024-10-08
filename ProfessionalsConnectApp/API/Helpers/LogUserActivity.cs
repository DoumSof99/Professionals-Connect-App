using System;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultsContext = await next();

        if(context.HttpContext.User.Identity?.IsAuthenticated != true) return;

        var username = resultsContext.HttpContext.User.GetUsername();
        var repo = resultsContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
        var user = await repo.GetUserByUsernameAsync(username);
        if(user == null) return;
        user.LastActive = DateTime.Now;
        await repo.SaveAllAsync();
    }
}
