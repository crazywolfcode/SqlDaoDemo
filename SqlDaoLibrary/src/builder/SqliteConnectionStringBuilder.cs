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
        /// <param name="dbname">数据库名称 如：*.db</param>
        /// <param name="password">密码</param>
        /// <param name="polling">是否启用连接线程池，默认为启用</param>
        /// <returns></returns>
        public static String BuildConnectionString(string dbname,string password = "" ,bool polling=true)
        {
            string SqlConnStrTemplate = $" Data Source={0};Version=3;Pooling={1};";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data") + "\\" + dbname;
            if (string.IsNullOrWhiteSpace(password))
            {
                return string.Format(SqlConnStrTemplate, path, polling);
            }
            else
            {
                return string.Format(SqlConnStrTemplate, path, polling) + ";Password=" + password + ";";
            }
           
        }
        /// <summary>
        /// 构建 SQLite 数据库连接字符串
        /// </summary>
        /// <param name="dbFile">数据库文件包含路径 如 D:/data/data.db</param>
        /// <param name="password">密码</param>
        /// <param name="polling">是否启用连接线程池，默认为启用</param>
        /// <returns></returns>
        public static String BuildConnectionStringWithDbfile(string dbFile,string password ="", bool polling = true)
        {
            string SqlConnStrTemplate = $" Data Source={0};Version=3;Pooling={1};";           
          
            if (string.IsNullOrWhiteSpace(password))
            {
                return string.Format(SqlConnStrTemplate, dbFile, polling);
            }
            else
            {
                return string.Format(SqlConnStrTemplate, dbFile, polling)+ ";Password=" + password + ";";
            }

        }
    }
}
