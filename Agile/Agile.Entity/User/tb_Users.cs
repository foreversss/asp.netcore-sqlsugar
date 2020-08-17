using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Agile.Entity.User
{
    [SugarTable("tb_Users")]
    public class tb_Users
    {
        //指定主键和自增列
        [SugarColumn(IsPrimaryKey = true,IsIdentity = true)]
        public int Id { get; set; } 
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public bool Sex { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreationTime { get; set; }

    }
}
