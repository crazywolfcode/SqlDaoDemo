using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDao
{
    /// <summary>
    /// DataBase Schema
    /// </summary>
    public class DbSchema
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 注解
        /// </summary>
        public string TableComment { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public string UpdateTime { get; set; }
        /// <summary>
        /// 数据行数
        /// </summary>
        public string TableRows { get; set; }
        /// <summary>
        /// 数据长度
        /// </summary>
        public string DataLength { get; set; }
    }
}
