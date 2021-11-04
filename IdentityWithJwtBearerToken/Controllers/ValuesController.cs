using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityWithJwtBearerToken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // importante
    public class ValuesController : ControllerBase
    {
        [HttpGet("getFruits")]
        [AllowAnonymous]
        public ActionResult GetFruits()
        {
            List<string> myList = new List<string>() { "apples", "bannanas" };
            return Ok(myList);
        }

        [HttpGet("getFruitsAuthenticated")]
        public ActionResult GetFruitsAuthenticated()
        {
            List<string> myList = new List<string>() { "organic apples", "organic bannanas" };
            return Ok(myList);
        }

        [AllowAnonymous]
        [HttpPost("getToken")]
        public async Task<ActionResult> GetToken([FromBody] MyCredentialsModel myCredentialsModel)
        {
            if (myCredentialsModel.Email == "emerson@emerson.com" && myCredentialsModel.Password == "mypass123")
            {

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("MY_BIG_SECRET_KEY_lkdjflkdjfklj¨&@*(@*$()#)()@(#)@(HJB<MBSDKM<BNASD");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, myCredentialsModel.Email)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature),
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString =  "Bearer " + tokenHandler.WriteToken(token);
                return Ok(new { Token = tokenString });
            }
            else
            {
                return Unauthorized("Login ou senha inválido. Tente novamente.");
            }
        }
    }
}
