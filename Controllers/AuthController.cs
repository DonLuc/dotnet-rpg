using System.Threading.Tasks;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController: ControllerBase
    {
        public readonly IAuthRepository _authRepo;
        public AuthController(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterDto request)
        {
            ServiceResponse<int> response = await _authRepo.Register(
                new Models.User { 
                    Username = request.Username
                }, request.Passoword
            );
            if(!response.Success) {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserRegisterDto request)
        {
            ServiceResponse<string> response = await _authRepo.Login(
                request.Username, request.Passoword
            );
            if(!response.Success) {
                return BadRequest(response);
            }
            return Ok(response);
        }
        
    }
}