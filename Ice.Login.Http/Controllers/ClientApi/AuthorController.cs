﻿using Common.Error;
using Common.Model;
using Ice.Login.Http.Controllers.Base;
using Ice.Login.Service.Service.ClientService.UserMng;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ice.Login.Http.Controllers.ClientApi;

/// <summary>
/// </summary>
/// <param name="userService"></param>
public class AuthorController(IUserService userService) : BaseController
{
    /// <summary>
    ///     TestGet
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ApiResult> Get()
    {
        throw new KnownException("Test", "Test", 500);
    }

    /// <summary>
    ///     注册账号
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ApiResult> Register([FromBody] RegisterAccountRequest body)
    {
        var data = await userService.RegisterAccount(body);
        return Ok(data);
    }

    /// <summary>
    ///     登录接口
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ApiResult> Login([FromBody] LoginRequest body)
    {
        var data = await userService.Login(body);
        return Ok(data);
    }

    /// <summary>
    ///     刷新token
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    // [TypeFilter(typeof(SessionValidationFilter))] // 使用过滤器验证会话
    public async Task<ApiResult> RefreshToken([FromBody] RefreshTokenRequest body)
    {
        var data = await userService.RefreshToken(body.RefreshToken);
        return Ok(data);
    }
}