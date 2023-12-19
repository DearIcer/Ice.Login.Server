using Common.Model;
using Ice.Login.Http.Controllers.Base;
using Ice.Login.Repository.IRepository.ClientRepository;
using Microsoft.AspNetCore.Mvc;

namespace Ice.Login.Http.Controllers.ClientApi
{
    public class ClientApiController : BaseController
    {
        private readonly IUserInfoRepository _userInfoRepository;
        public ClientApiController(IUserInfoRepository userInfoRepository)
        {
            _userInfoRepository = userInfoRepository;
        }
        /// <summary>
        /// TestGet
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult> Get()
        {
            var data = await _userInfoRepository.Queryable(it => it.Id >= 1);
            return Response(data);
            //return Response("Hello World");
        }

    }
}
