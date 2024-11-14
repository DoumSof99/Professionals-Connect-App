using System;
using API.Data;
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

        var userId = resultsContext.HttpContext.User.GetUserId();
        var unitOfWork = resultsContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
        var user = await unitOfWork.UserRepository.GetUserByIdAsync(userId);
        
        if(user == null) return;
        user.LastActive = DateTime.Now;
        await unitOfWork.Complete();
    }
}
