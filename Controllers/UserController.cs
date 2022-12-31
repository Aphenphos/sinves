using Microsoft.AspNetCore.Mvc;
using sinves.Models;
using sinves.Services;
using System.Security.Cryptography;

namespace sinves.Controllers

{

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserService _userService;
        public UserController(UserService userService) =>
            _userService = userService;

        [HttpPost("signup")]
        public async Task<IActionResult> Post(UserDto newUser)
        {
            User user = new User();
            CreatePasswordHash(newUser.password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordSalt= passwordSalt;
            user.PasswordHash= passwordHash;
            user.Username= newUser.username;

            await _userService.CreateAsync(user);

            return CreatedAtAction(nameof(Post), new { id = user.Id }, user.Username);
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(UserDto userInfo)
        {

            User fromDB =  _userService.GetAsync(userInfo.username).Result;
            
            if (!(userInfo.username == fromDB.Username))
            {
                return BadRequest("Invalid Username");
            }

            if(VerifyPasswordHash(userInfo.password, fromDB.PasswordHash, fromDB.PasswordSalt) == true)
            {
                return Ok("Token");
            } else
            {
                return BadRequest("Invalid Login");
            }
        }


        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) 
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt )
        {
            using(var hmac = new HMACSHA512(passwordSalt)) 
            {
                var testHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return testHash.SequenceEqual(passwordHash);
            }

        }
    }
}
