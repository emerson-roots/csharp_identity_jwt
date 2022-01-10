using IdentityWithJwtBearerToken.Areas.Identity.Data;
using IdentityWithJwtBearerToken.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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


    /*
     * video aula https://www.youtube.com/watch?v=Lh82WlOvyQk&list=LL&index=4&t=858s
     * 
     * 
     * **/
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // importante
    public class ValuesController : ControllerBase
    {

        private IdentityWithJwtBearerTokenContext _dbContext;

        // IdentityWithJwtBearerTokenUser é como se fosse uma MODEL especifica para o IdentityUser
        // ela herda de Identity portanto herda os atributos de credenciais basicos
        private readonly UserManager<IdentityWithJwtBearerTokenUser> _userManager;
        private readonly SignInManager<IdentityWithJwtBearerTokenUser> _signInManager;

        public ValuesController(IdentityWithJwtBearerTokenContext dbContext, 
            UserManager<IdentityWithJwtBearerTokenUser> userManager, 
            SignInManager<IdentityWithJwtBearerTokenUser> signInManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }

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
            // 30:33
            // captura o usuario no banco de dados
            // neste projeto esta sendo usado o EntityFramework
            // adaptar para uso de DAPPER
            //
            var user = _dbContext.Users.FirstOrDefault(x => x.Email == myCredentialsModel.Email);

            if (user != null)
            {
                var signInResult = await _signInManager.CheckPasswordSignInAsync(user, myCredentialsModel.Password, false);

                if (signInResult.Succeeded)
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
                    var tokenString = "Bearer " + tokenHandler.WriteToken(token);
                    return Ok(new { Token = tokenString });
                }
                else
                {
                    // no video ele utilizou OK, eu substitui por Unauthorized
                    return Unauthorized("Login ou senha inválido. Tente novamente.");
                }
            }
            else
            {
                return Unauthorized("Login ou senha inválido. Tente novamente.");
            }

        }


        /*
         * IMPORTANTE:
         * como a classe IdentityWithJwtBearerTokenUser herda de IdentityUser
         * é obrigatório usar padroes de segurança para SENHAS
         * como por exemplo letras maiusculas, numeros etc
         * 
         * isso é ótimo pois não precisa implementar na mão
         * 
         * **/
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] MyCredentialsModel myCredentialsModel)
        {
            // 35:20 - no video ele recomendou uso de TRY/CATCHS para capturar
            // ou progpagar erros de acordo
            // mas não utilizou para ser mais direto ao ponto



            IdentityWithJwtBearerTokenUser identityWithJwtBearerTokenUser = new IdentityWithJwtBearerTokenUser()
            {
                Email = myCredentialsModel.Email,
                UserName = myCredentialsModel.Email, // pode ser um nome de usuario comum, nao necessariamente um email
                EmailConfirmed = false // 37:00 -  interessante, para envio de e-mails de confirmação, como um codigo de confirmação por exemplo
            };

            // salva no banco de dados (neste caso, usamos o SQL server do visual studio)
            //
            // IMPORTANTE
            // ele nao salva a senha bruta, ele salva em HASH
            // para o primeiro primeiro teste exemplo, foi usado as seguintes credenciais
            // login: teste_novo_cadastro@email
            // senha: abc123ABC*

            var result = await _userManager.CreateAsync(identityWithJwtBearerTokenUser, myCredentialsModel.Password);

            if (result.Succeeded)
            {
                return Ok(new { Result = "Registrado com sucesso" });
            }
            else
            {

                // em caso de falhas
                // percorre os erros do resultado para demonstrar
                // ao usuario o que deu errado
                StringBuilder stringBuilder = new StringBuilder();

                foreach (var error in result.Errors)
                {
                    stringBuilder.Append(error.Description);
                    //stringBuilder.Append("\r\n");
                }
                return BadRequest(new { Result = $"Falha no registro: { stringBuilder.ToString() }" });
            }


        }

    }
}
