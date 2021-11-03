using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
