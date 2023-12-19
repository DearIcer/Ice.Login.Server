using Common.Model;
using Ice.Login.Http.Controllers.Base;
using Ice.Login.Repository.IRepository.ClientRepository;
using Ice.Login.Service.Service.ClientService.UserMng;
using Microsoft.AspNetCore.Mvc;

namespace Ice.Login.Http.Controllers.ClientApi
{
    public class ClientApiController : BaseController
    {
        private readonly IUserService _userService;
        public ClientApiController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// TestGet
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult> Get()
        {
            var data = await _userService.Queryable();
            return Response(data);
            //return Response("Hello World");
        }

        /// <summary>
        /// TestGet
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult> Register()
        {
            var data = await _userService.Queryable();
            return Response(data);
            //return Response("Hello World");
        }
    }
}
