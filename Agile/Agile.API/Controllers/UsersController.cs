using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Agile.BaseLib.Helpers;
using Agile.BaseLib.Models;
using Agile.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NPOI.OpenXmlFormats.Dml;

namespace Agile.API.Controllers
{ 
    
    public class UsersController : BaseController
    {
        private readonly IConfiguration _configuration;

        private readonly IUsersRepository _usersRepository;


        public UsersController(IConfiguration configuration, IUsersRepository usersRepository)
        {
            _configuration = configuration;
            _usersRepository = usersRepository;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="User">用户账号 密码</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ExcutedResult>> UserLogin(dynamic User)
        {
            //api执行执行状态
            int status = (int)ExcutedResult.status.成功;
            //返回消息           
            string msg = ExcutedResult.GetDescription(ExcutedResult.status.成功);

            try
            {
                //用户名
                string username = User.UserName;
                //用户密码
                string password = User.Password;

                var user = await _usersRepository.QueryByEntity(x => x.UserName == username && x.Password == password);

                if (user == null || user.Id <= 0)
                {
                    status = (int)ExcutedResult.status.账号密码错误;
                    msg = ExcutedResult.GetDescription(ExcutedResult.status.账号密码错误);

                    return ExcutedResult.FailedResult(msg, status);
                }

                JwtTokenUtil jwtTokenUtil = new JwtTokenUtil(_configuration);

                string token = jwtTokenUtil.GetToken(user);
                return ExcutedResult.SuccessResult(token);
            }
            //异常处理
            catch (Exception ex)
            {
                msg = ex.Message;

                status = (int)ExcutedResult.status.API内部错误;

                return ExcutedResult.FailedResult(msg, status);
            }   
        }

        /// <summary>
        /// 用户列表
        /// </summary>
        /// <returns></returns>    
        [HttpGet]
        public async Task<ActionResult<ExcutedResult>> GetAllUsers()
        {
            //api执行执行状态
            int status = (int)ExcutedResult.status.成功;
            //返回消息           
            string msg = ExcutedResult.GetDescription(ExcutedResult.status.成功);

            try
            {
                //用户列表
                var userlist = await _usersRepository.Query();

                //获取token里自定义参数值
                //var auth = HttpContext.AuthenticateAsync();
                //var username = auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.NameIdentifier))?.Value;

                //判断是否有数据
                if (userlist == null || userlist.Count <= 0)
                {
                    msg = ExcutedResult.GetDescription(ExcutedResult.status.API无数据);

                    status = (int)ExcutedResult.status.API无数据;

                    return ExcutedResult.FailedResult(msg, status);
                }

                return ExcutedResult.SuccessResult(msg, userlist, status);
            }
            //异常处理
            catch (Exception)
            {
                msg = ExcutedResult.GetDescription(ExcutedResult.status.API内部错误);

                status = (int)ExcutedResult.status.API内部错误;

                return ExcutedResult.FailedResult(msg, status);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public string GetRedis()
        {

            var redisResult = RedisHelper.redisClient.GetStringKey("frist");

            return redisResult;
        }
        [AllowAnonymous]
        [HttpGet]
        public string SetRedis()
        {          
            var s = RedisHelper.redisClient.SetStringKey("Token", "1wds123a", new TimeSpan(0,0,0,10));
            return "OK";
        }
    }
}
