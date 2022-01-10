using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace IdentityWithJwtBearerToken.Areas.Identity.Data
{

    /*
     * video aula https://www.youtube.com/watch?v=Lh82WlOvyQk&list=LL&index=4&t=858s
     * 
     * 
     * **/

    /*
     * 
     * 
     * IdentityWithJwtBearerTokenUser é como se fosse uma MODEL especifica para o IdentityUser
     * ela herda de Identity portanto herda os atributos de credenciais basicos
     * 
     * 
     * o interessante é que em caso de gerar novos registros
     * ele ja possui alguns padrões de senha por exemplo
     * como requerer letras maiusculas
     * requerer números
     * etc
     * **/


    // Add profile data for application users by adding properties to the IdentityWithJwtBearerTokenUser class
    public class IdentityWithJwtBearerTokenUser : IdentityUser
    {
    }
}
