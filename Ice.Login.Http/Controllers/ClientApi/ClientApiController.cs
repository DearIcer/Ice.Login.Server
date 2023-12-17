using Ice.Login.Http.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Ice.Login.Http.Controllers.ClientApi
{
    public class ClientApiController : BaseController
    {
        /// <summary>
        /// TestGet
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Json("aaa"); 
        }

    }
}
