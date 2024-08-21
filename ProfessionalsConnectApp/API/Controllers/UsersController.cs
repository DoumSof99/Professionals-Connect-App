using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")] // localhost:5001/api/users
public class UsersController(DataContext context) : ControllerBase 
{
    [HttpGet]
    public ActionResult<IEnumerable<AppUser>> GetUsers(){
        var users = context.Users.ToList();
        return users;
    }

    [HttpGet("{id:int}")]  // /api/users/1
    public ActionResult<AppUser> GetUser(int id){
        var user = context.Users.Find(id);
        if(user == null) return NotFound();
        return user;
    }
}
