using COeX_India1._0.Data;
using COeX_India1._0.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace COeX_India1._0.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]

    public class UsersController:Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _config;
        public UsersController(IConfiguration config, ApplicationDbContext context)
        {
            _dbContext = context;
            _config= config;
        }

        private string GenerateToken(User user)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                 {
                    new Claim(JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("userId", user.UserId.ToString()),
                    new Claim("userType", user.UserType.ToString())
                 };

                var token = new JwtSecurityToken
                    (
                    _config["Jwt:Issuer"],
                    _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddYears(12),
                    signingCredentials: credentials
                     
                    );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch(Exception ex)
            {
                return string.Empty;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> loginCluster(ClusterLoginModel loginModel)
        {
            try
            {
                if(loginModel == null) { return BadRequest(new Response(false, "Plz enter credentials")); }
                if (loginModel.ClusterId == 0) { return BadRequest(new Response(false, "Plz enter a valid Cluster Id")); }
                if (string.IsNullOrWhiteSpace(loginModel.Username)) { return BadRequest(new Response(false, "Plz enter a valid username")); }
                if(string.IsNullOrWhiteSpace(loginModel.Password)) { return BadRequest(new Response(false, "Plz enter a valid Password")); }

                var cluster= await _dbContext.Clusters.AsNoTracking().Where(c=> c.Id == loginModel.ClusterId && c.Passcode == loginModel.passcode).FirstOrDefaultAsync();
                if(cluster == null) { return BadRequest(new Response(false, "cluster or passcode invalid")); }

                var user=await _dbContext.Users.AsNoTracking().Where(c => c.Username == loginModel.Username && c.Password == loginModel.Password && c.UserType == 0).FirstOrDefaultAsync();
                if(user == null)
                {
                    return BadRequest(new Response(false, "Username or password invalid"));
                }

                var token = GenerateToken(user);
                if (token == string.Empty) { return BadRequest(new Response(false, "Please Try Again In a While")); }

                return Ok(new LoginResponse(true, token));

            }
            catch(Exception ex)
            {
                return Problem("Oops! plz try again later");
            }
        }
    }
}
