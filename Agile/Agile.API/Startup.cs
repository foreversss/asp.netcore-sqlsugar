using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agile.BaseLib.Extension;
using Agile.BaseLib.Ioc;
using Agile.BaseLib.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text.Json;
using System.IO;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Agile.BaseLib.Models;
using Agile.BaseLib.Helpers;

namespace Agile.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //序列化json的首字母小写处理
            services.AddMvc().AddNewtonsoftJson(opt =>
            {
                //忽略循环引用
                opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //不改变字段大小
                opt.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            //注册Swagger生成器，定义一个和多个Swagger 文档
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Agile API", Version = "v1" });
                // 为 Swagger JSON and UI设置xml文档注释路径
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                var xmlPath = Path.Combine(basePath, "Agile.API.xml");
                c.IncludeXmlComments(xmlPath);
            });


            //跨域
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {
                    builder.AllowAnyOrigin() //允许任何来源的主机访问                  
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();//指定处理cookie
                });
            });

            //获取连接字符串
            services.Configure<DbContextOption>(Configuration.GetSection("ConnectionStrings"));

            //依赖注入
            services.AddScopedAssembly("Agile.Repository");
          
            //生成Token
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = true,//是否验证Issuer
                         ValidateAudience = true,//是否验证Audience
                         ValidateLifetime = true,//是否验证失效时间
                         ValidateIssuerSigningKey = true,//是否验证SecurityKey                       
                         AudienceValidator=(m,n,z) => { return m != null && m.FirstOrDefault().Equals(Configuration["ValidAudience"]); },//这里采用动态验证的方式，在重新登陆时，刷新token，旧token就强制失效了
                         ValidIssuer = "igbom_web",//Issuer，这两项和前面签发jwt的设置一致
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecurityKey"]))//拿到SecurityKey
                     };
                 });

            //Redis服务依赖
            RedisHelper.redisClient.InitConnect(Configuration);

            return AspectCoreContainer.BuildServiceProvider(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //启用jwt验证
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Agile API V1");
            });           
        }
    }
}
