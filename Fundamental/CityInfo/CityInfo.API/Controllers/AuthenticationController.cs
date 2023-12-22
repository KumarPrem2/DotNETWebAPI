using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CityInfo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public class AuthenticationRequestBody
        {
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }

        private class CityInfoUser
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string City { get; set; }

            public CityInfoUser(
                int userId,
                string userName,
                string firstName,
                string lastName,
                string city)
            {
                UserId = userId;
                UserName = userName;
                FirstName = firstName;
                LastName = lastName;
                City = city;
            }
        }

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(AuthenticationRequestBody authenticationRequestBody)
        {
            // step1. Validate username and password
            var user = ValidateUserCredentials(authenticationRequestBody.UserName, authenticationRequestBody.Password);
            if (user == null)
            {
                return Unauthorized();
            }

            // step2. Create a token
            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"]));

            var signingCredentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);
            var claimsForToken = new List<Claim>()
            {
                new Claim("sub", user.UserId.ToString()),
                new Claim("givn_name", user.FirstName),
                new Claim("family_name", user.LastName),
                new Claim("city", user.City)
            };

            var jwtSecurityTokens = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler()
                                    .WriteToken(jwtSecurityTokens);
            return Ok(tokenToReturn);

        }

        private CityInfoUser ValidateUserCredentials(string? userName, string? password)
        {
            return new CityInfoUser(1, 
                userName ?? "",
                "Kevin",
                "Dockx",
                "Antwerp");
        }
    }
}
