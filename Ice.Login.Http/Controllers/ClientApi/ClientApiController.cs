using Common.Error;
using Common.Model;
using Ice.Login.Http.Controllers.Base;
using Ice.Login.Service.Service.ClientService.UserMng;
using Microsoft.AspNetCore.Mvc;

namespace Ice.Login.Http.Controllers.ClientApi
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userService"></param>
    public class ClientApiController(IUserService userService) : BaseController
    {
        private readonly IUserService _userService = userService;

        /// <summary>
        /// TestGet
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult> Get()
        {
            throw new KnownException("Test", "Test", 500);
            return Response("Hello World");
        }

        /// <summary>
        /// 注册账号
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult> Register([FromBody] RegisterAccountRequest body)
        {
            var data = await _userService.RegisterAccount(body);
            return Response(data);
        }
    }
}
