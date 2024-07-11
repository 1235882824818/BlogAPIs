using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BlogAPIs.VM;
using BlogAPIs.Entities;
using BlogAPIs.Services;

namespace BlogAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticatorController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IUserService userService;

        public AuthenticatorController(UserManager<User> userManager, IConfiguration configuration, IUserService userService)
        {
            _userManager = userManager;
            _configuration = configuration;
            this.userService = userService;
        }

        [HttpPost("RegisterAsync")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

                var result = await userService.RegisterUserAsync(model);

                if (!result.isAuthenticated)
                        return BadRequest(result.Message);
                   
            return Ok(result);
        }

        [HttpPost("LoginAsync")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await userService.LoginUserAsync(model);

            if (!result.isAuthenticated)
                return BadRequest(result.Message);

            return Ok(new {result.Roles, result.Token, result.ExpirationDate});
        }


    }
}