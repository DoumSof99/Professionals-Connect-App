﻿using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers {

    [Authorize]
    public class UsersController : BaseApiController {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet] // api/users
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers() { 
            var users = await _context.Users.ToListAsync();
            return users;
        }

        [HttpGet("{id}")] // api/users/2
        public async Task<ActionResult<AppUser>> GetUserByID(int id) { 
            var user = await _context.Users.FindAsync(id);
            if (user is not null) {
                return user;
            }
            return NoContent();
        }

        //[HttpPut("{id, user}")]
    }
}
