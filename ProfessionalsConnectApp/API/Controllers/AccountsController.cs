using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers {
    public class AccountsController : BaseApiController {
        private readonly DataContext _context;

        public AccountsController(DataContext context) {
            _context = context;
        }

        [HttpPost("register")]  // api/accounts/register
        public async Task<ActionResult<AppUser>> RegisterUser(RegisterDTO registerDTO) {

            if (await UserExists(registerDTO.UserName)) {
                return BadRequest("Username is taken");
            }

            using var hmac = new HMACSHA512();

            var user = new AppUser {
                UserName = registerDTO.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> LoginUser(LoginDTO loginDTO) {

            var user = await _context.Users.FirstOrDefaultAsync(p => p.UserName == loginDTO.UserName.ToLower());

            if (user is null) return Unauthorized("Invalid Username");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for (int i = 0; i < computedHash.Length; i++) {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }

            return Ok(user);
        }

        private async Task<bool> UserExists(string username) {
            return await _context.Users.AnyAsync(u => u.UserName == username.ToLower());
        }
    }
}
