using System;
using System.Collections.Generic;
using System.Text;

namespace Agile.BaseLib.Options
{
    public class DbContextOption
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string MySql { get; set; }

        /// <summary>
        /// 是否在控制台输出SQL语句，默认开启
        /// </summary>
        public bool IsOutputSql { get; set; } = true;
    }
}
