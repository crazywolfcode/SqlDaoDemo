using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDao
{
    /// <summary>
    /// Mysql Tabe Schema
    /// </summary>
    public class MysqlTabeSchema : TableScema
    {
        /// <summary>
        /// 字段
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 空
        /// </summary>
        public string Null { get; set; }
        /// <summary>
        /// 是否主键
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string Default { get; set; }
        /// <summary>
        /// Extra 字段
        /// </summary>
        public string Extra { get; set; }
    }
}
