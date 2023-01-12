using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using sinves.Models;
using sinves.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace sinves.Controllers

{

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
            {
            _userService = userService;
            }


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

            try
            {
            User fromDB= _userService.GetAsync(userInfo.username).Result;
                if (fromDB != null)
                {
                    if (!(userInfo.username == fromDB.Username))
                    {
                        return BadRequest("Invalid Username");
                    }

                    if (VerifyPasswordHash(userInfo.password, fromDB.PasswordHash, fromDB.PasswordSalt) == true)
                    {
                        var token = GenerateJwtToken(fromDB);
                        Response.Cookies.Append("JWT", token, new CookieOptions()
                        {
                            HttpOnly = true,
                            //Change samesite mode in prod
                            SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
                            Secure= true
                        }) ;

                        
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Invalid Login");
                    }
                } else
                {
                    throw new Exception("Invalid Login");
                }

            } catch(Exception ex)
            {
                return BadRequest("Authentication Error");
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

        private string GenerateJwtToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            byte[] key = Encoding.UTF8.GetBytes("wmuLnCwLvGKtC0NDAPkQ");

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                    new Claim(JwtRegisteredClaimNames.Name, value:user.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
                }),

                Expires = DateTime.Now.AddHours(1).ToUniversalTime(),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

       return jwtToken;
        }
    }
}
