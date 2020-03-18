using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDao
{
    /// <summary>
    /// Sqlite Table Schema
    /// </summary>
    public class SqliteTableSchema : TableScema
    {
        /// <summary>
        /// 字段
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 可空
        /// </summary>
        public string Notnull { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string Dflt_value { get; set; }
        /// <summary>
        /// 是否主键
        /// </summary>
        public string Pk { get; set; }
    }
}
