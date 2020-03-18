using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDao
{
    /// <summary>
    /// MysqlTableColumnSchema
    /// </summary>
    public class MysqlTableColumnSchema
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// 是否可以空
        /// </summary>
        public string IsNullable { get; set; }
        /// <summary>
        /// 注解
        /// </summary>
        public string CommentValue { get; set; }
    }
}
