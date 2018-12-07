using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository repo;

        public AuthController(IAuthRepository repo){
            this.repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserToRegisterDTO userToRegister){

            //username = 
            if(await repo.UserExists(userToRegister.Username)){
                return BadRequest("User with username exists.");
            }

            var user = new User{
                Username = userToRegister.Username
            };

            await repo.Register(user,userToRegister.Password);

            return StatusCode(201);

        }
    }
}