using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SqlDao
{
    /// <summary>
    /// Sqlite Connection String Builder
    /// </summary>
    public class SqliteConnectionStringBuilder : SqlBuilder
    {

        /// <summary>
        /// 构建 SQLite 数据库连接字符串
        /// 如：Data Source=./data/data.db;Version=3;Pooling=False;Max Pool Size=100;
        /// </summary>
        /// <param name="dbname">数据库名称 *.db</param>
        /// <returns></returns>
        public static String BuildConnectionString(string dbname)
        {
            string SqlConnStrTemplate = " Data Source={0};Version=3;Pooling=False;Max Pool Size=100;";
            return string.Format(SqlConnStrTemplate, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data")) + "\\" + dbname;
        }
    }
}
