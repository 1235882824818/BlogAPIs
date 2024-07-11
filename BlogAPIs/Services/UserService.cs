using BlogAPIs.Entities;
using BlogAPIs.VM;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlogAPIs.Services
{
    public interface IUserService
    {
        Task<UserManagerResponse> RegisterUserAsync(RegisterModel model);
        Task<UserManagerResponse> LoginUserAsync(LoginModel model);
    }
    public class UserService : IUserService
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;


        public UserService(UserManager<User> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;

        }

        public async Task<UserManagerResponse> RegisterUserAsync(RegisterModel model)
        {
            var useremail = await userManager.FindByEmailAsync(model.Email);
            var username = await userManager.FindByNameAsync(model.Username);

            if (useremail != null)
                return new UserManagerResponse { Message = $"{model.Email} is Allready Registered!" };
            else if (username != null)
                return new UserManagerResponse { Message = $"{username} is Used, Please Try Another Name " };
            else
            {
                if (model == null)
                    throw new ArgumentNullException("Please Input Valid Data");

                if (model.Password != model.ConfirmPassword)
                {
                    return new UserManagerResponse
                    {
                        Message = "The Password Doesn't Match in Confirm Password Field"
                    };

                }
                var User = new User
                {
                    UserName = model.Username,
                    Email = model.Email
                };

                var result = await userManager.CreateAsync(User, model.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(User, "User");
                    var JwtSecurityToken = await GenerateJwtToken(User);

                    return new UserManagerResponse
                    {
                        Message = "User Registerd Succesfully!",
                        isAuthenticated = true,
                        Roles = new List<string> { "User" },
                        Email = model.Email,
                        Username = model.Username,
                        ExpirationDate = JwtSecurityToken.ValidTo,
                        Token = new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken)
                    };

                }

                return new UserManagerResponse
                {
                    Message = $"Failed To Register, {result.Errors.Select(f => f.Description)}"
                };
            }
        }

        private async Task<JwtSecurityToken> GenerateJwtToken(User user)
        {
            var userClaims = await userManager.GetClaimsAsync(user);
            var roles = await userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        public async Task<UserManagerResponse> LoginUserAsync(LoginModel model)
        {
            var userResponse = new UserManagerResponse();

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user is null || !await userManager.CheckPasswordAsync(user, model.Password) )
                return new UserManagerResponse { Message = "Email or Password is incorrect" };

            var JwtSecurityToken = await GenerateJwtToken(user);
            var rolesList = await userManager.GetRolesAsync(user);

            userResponse.Username = user.UserName;
            userResponse.Email = user.Email;
            userResponse.isAuthenticated = true;
            userResponse.ExpirationDate = DateTime.Now;
            userResponse.Token = new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken);
            userResponse.Roles = rolesList.ToList();

            return userResponse;
        }

    }
}