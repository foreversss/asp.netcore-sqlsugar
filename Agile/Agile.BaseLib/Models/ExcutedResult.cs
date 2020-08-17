using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Agile.BaseLib.Models
{
    public class ExcutedResult
    {

        public bool success { get; set; }

        public string msg { get; set; }
        public int code { get; set; }

        public object rows { get; set; }

        public enum status
        {
            成功 = 200,       
            账号密码错误 = 402,
            无访问权限或票据过期 = 401,
            API内部错误 = 500,
            API无数据 = 204
        }

        public ExcutedResult(bool success, string msg, object rows,int code)
        {
            this.success = success;
            this.msg = msg;
            this.rows = rows;
            this.code = code;
        }
        public ExcutedResult()
        {

        }
        public static ExcutedResult SuccessResult(string msg = null, int code = 0)
        {
            return new ExcutedResult(true, msg, null, code);
        }
        public static ExcutedResult SuccessResult(object rows, int code)
        {
            return new ExcutedResult(true, null, rows,0);
        }
        public static ExcutedResult SuccessResult(string msg, object rows, int code)
        {
            return new ExcutedResult(true, msg, rows, code);
        }
        public static ExcutedResult FailedResult(string msg, int code)
        {
            return new ExcutedResult(false, msg, null, code);
        }


        /// <summary>
        /// 获取枚举的描述
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static string GetDescription<TEnum>(TEnum @enum)
        {
            FieldInfo fieldInfo = @enum.GetType().GetField(@enum.ToString());
            DescriptionAttribute[] descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : @enum.ToString();
        }
    }

    public class PaginationResult : ExcutedResult
    {
        /// <summary>
        /// 总条数
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 每页条数
        /// </summary>
        public int pageSize { get; set; }
        /// <summary>
        /// 当前页码
        /// </summary>
        public int pageIndex { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int pageCount => total % pageSize == 0 ? total / pageSize : total / pageSize + 1;

        public PaginationResult(bool success, string msg, object rows, int code) : base(success, msg, rows, code)
        {
        }

        public static PaginationResult PagedResult(object rows, int total, int size, int index,int code)
        {
            return new PaginationResult(true, null, rows, code)
            {
                total = total,
                pageSize = size,
                pageIndex = index
            };
        }
    }



}
