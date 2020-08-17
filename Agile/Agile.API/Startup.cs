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
            //���л�json������ĸСд����
            services.AddMvc().AddNewtonsoftJson(opt =>
            {
                //����ѭ������
                opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //���ı��ֶδ�С
                opt.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            //ע��Swagger������������һ���Ͷ��Swagger �ĵ�
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Agile API", Version = "v1" });
                // Ϊ Swagger JSON and UI����xml�ĵ�ע��·��
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//��ȡӦ�ó�������Ŀ¼�����ԣ����ܹ���Ŀ¼Ӱ�죬������ô˷�����ȡ·����
                var xmlPath = Path.Combine(basePath, "Agile.API.xml");
                c.IncludeXmlComments(xmlPath);
            });


            //����
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {
                    builder.AllowAnyOrigin() //�����κ���Դ����������                  
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();//ָ������cookie
                });
            });

            //��ȡ�����ַ���
            services.Configure<DbContextOption>(Configuration.GetSection("ConnectionStrings"));

            //����ע��
            services.AddScopedAssembly("Agile.Repository");
          
            //����Token
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = true,//�Ƿ���֤Issuer
                         ValidateAudience = true,//�Ƿ���֤Audience
                         ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��
                         ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey                       
                         AudienceValidator=(m,n,z) => { return m != null && m.FirstOrDefault().Equals(Configuration["ValidAudience"]); },//������ö�̬��֤�ķ�ʽ�������µ�½ʱ��ˢ��token����token��ǿ��ʧЧ��
                         ValidIssuer = "igbom_web",//Issuer���������ǰ��ǩ��jwt������һ��
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecurityKey"]))//�õ�SecurityKey
                     };
                 });

            //Redis��������
            RedisHelper.redisClient.InitConnect(Configuration);

            return AspectCoreContainer.BuildServiceProvider(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //����jwt��֤
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
            //�����м�������swagger-ui��ָ��Swagger JSON�ս��
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Agile API V1");
            });           
        }
    }
}
