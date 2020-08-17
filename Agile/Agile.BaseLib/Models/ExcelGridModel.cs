using System;
using System.Collections.Generic;
using System.Text;

namespace Agile.BaseLib.Models
{
    public class ExcelGridModel
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// excel列名
        /// </summary>
        public string label { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        public string width { get; set; }
        /// <summary>
        /// 对其方式
        /// </summary>
        public string align { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public string height { get; set; }
        /// <summary>
        /// 是否隐藏
        /// </summary>
        public string hidden { get; set; }
    }
}
