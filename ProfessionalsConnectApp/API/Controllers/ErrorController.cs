using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ErrorController(DataContext context) : BaseApiController
{
    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetAuth(){
        return "some text";
    }

    [HttpGet("not-found")]
    public ActionResult<AppUser> GetNotFount(){
        var s = context.Users.Find(-1);
        if(s == null) return NotFound();
        return s;
    }

    [HttpGet("server-error")]
    public ActionResult<AppUser> GetServerError(){
        var s = context.Users.Find(-1) ?? throw new Exception("This is server error");
        return s;
    }

    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest(){
        return BadRequest("this is bad request");
    }
}
