using Common.Model;
using Ice.Login.Http.Controllers.Base;
using Ice.Login.Service.Service.ClientService.UserMng;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ice.Login.Http.Controllers.ClientApi;

public class UserInfoController(IUserService userService) : BaseController
{
    /// <summary>
    ///     获取账号列表
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ApiResult> GetUserList([FromBody] GetUserInfoListRequest body)
    {
        var data = await userService.QueryableList(body.PageIndex, body.PageSize, body.Query);
        return new ApiResult
        {
            Data = data.Item2,
            Message = "success"
        };
    }
}