using Agile.BaseLib.Cache;
using Agile.BaseLib.Extension;
using Agile.BaseLib.Helpers;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Agile.BaseLib.Attributes
{
    /// <summary>
    /// 缓存属性。
    /// <para>
    /// 在方法上标记此属性后，通过该方法取得的数据将被缓存。在缓存有效时间范围内，往后通过此方法取得的数据都是从缓存中取出的。
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MemoryCacheAttribute : AbstractInterceptorAttribute
    {
        /// <summary>
        /// 缓存有限期，单位：分钟
        /// </summary>
        public int Expiration { get; set; } = 2;
        public string CacheKey { get; set; } = null;

        private static readonly MethodInfo _taskResultMethod;

        private readonly IMemoryCache _cache = MemoryCacheManager.GetInstance();
        static MemoryCacheAttribute()
        {
            _taskResultMethod = typeof(Task).GetMethods().FirstOrDefault(p => p.Name == "FromResult" && p.ContainsGenericParameters);
        }

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        { 
            var parameters = context.ServiceMethod.GetParameters();
            //判断Method是否包含ref / out参数
            if (parameters.Any(it => it.IsIn || it.IsOut))
            {
                await next(context);
            }
            else
            {
                var key = string.IsNullOrEmpty(CacheKey)
                     ? new CacheKey(context.ServiceMethod, parameters, context.Parameters).GetMemoryCacheKey()
                     : CacheKey;
                //返回值类型
                var returnType = context.IsAsync()
                    ? context.ServiceMethod.ReturnType.GetGenericArguments().First()
                    : context.ServiceMethod.ReturnType;
                if (_cache.TryGetValue(key, out object value))
                {
                    context.ReturnValue = context.IsAsync()
                   ? _taskResultMethod.MakeGenericMethod(returnType).Invoke(null, new object[] { value })
                   : value;
                    return;
                }
                else
                {
                    await context.Invoke(next);
                    object returnValue = context.ReturnValue;
                    if (context.ServiceMethod.IsReturnTask())
                    {
                        returnValue = returnValue.GetPropertyValue("Result");
                    }
                    _cache.Set(key, returnValue, new MemoryCacheEntryOptions()
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Expiration)
                    });
                    //await next(context);
                }
            }
        }
    }
}
