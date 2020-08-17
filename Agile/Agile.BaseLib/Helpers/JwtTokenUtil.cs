using Agile.BaseLib.Models;
using Agile.Entity.User;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Agile.BaseLib.Helpers
{
    public class JwtTokenUtil
    {
        private readonly IConfiguration _configuration;

        public JwtTokenUtil(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //获得token
        public string GetToken(tb_Users userInfo)
        {
            _configuration["ValidAudience"] = userInfo.UserName + userInfo.Password + DateTime.Now.ToString();
            //把用户的名字推到一个声明中
            var claims = new[] {
                 new Claim(ClaimTypes.Name,userInfo.UserName)
                 //自定义参数
                 //new Claim(ClaimTypes.NameIdentifier,userInfo.UserName)
             };
            //使用密钥对令牌进行签名。此密钥将在您的API和任何需要检查令牌是否合法的对象之间共享
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecurityKey"]));//获取密钥


            //第一个参数是根据预先的二进制字节数组生成一个安全秘钥，就是密码，
            //第二个参数是编码方式
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);//凭证 ，根据密钥生成

            var token = new JwtSecurityToken(
               //颁发者
               issuer: "igbom_web",
               //接收者
               audience: _configuration["ValidAudience"],
               //自定义参数
               claims: claims,
               //过期时间
               expires: DateTime.Now.AddMinutes(100),//token的有效期  这里设置100分钟token就失效
               //签名证书
               signingCredentials: creds
           );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
