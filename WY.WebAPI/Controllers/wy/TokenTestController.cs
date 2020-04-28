using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UIDP.UTILITY.JWTHelper;

namespace WY.WebAPI.Controllers.wy
{
    [Produces("application/json")]
    [Route("TokenTest")]
    public class TokenTestController : Controller
    {
        [HttpGet("GetToken")]
        public IActionResult GetToken()
        {
            JWTHelper jWT = new JWTHelper();
            return Ok(jWT.CreateToken(null,20));
        }
    }
}