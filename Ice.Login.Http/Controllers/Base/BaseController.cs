using Common.Model;
using Microsoft.AspNetCore.Mvc;

namespace Ice.Login.Http.Controllers.Base;

[ApiController]
[Route("api/[controller]/[action]")]
public class BaseController : Controller
{
    protected ApiResult Response(object data = null, string message = null, string errorCode = null)
    {
        return new ApiResult
        {
            Data = data,
            Message = message,
            ErrorCode = errorCode
        };
    }
}