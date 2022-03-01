using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDao
{
    /// <summary>
    /// Mysql Connection String Builder
    /// </summary>
    public class MysqlConnectionStringBuilder : SqlBuilder
    {

        /// <summary>
        /// 构建连接字符串
        /// 如 ："Database=weightsys;Data Source=192.168.88.3;User Id=admin;Password=pwd;CharSet=utf8;port=33069";
        /// </summary>
        /// <param name="dataSource">数据源 | IP</param>
        /// <param name="db">数据库存名称</param>
        /// <param name="userId">用户名称</param>
        /// <param name="pwd">密码</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        [Obsolete("已经过时，请使用 GetConnectionString 方法代替", true)]
        public static String BuildConnectionString(string dataSource, string db, string userId, string pwd, string port)
        {
            String connectionStringTemplate = "Data Source={1};Database={0};User Id={2};Password={3};CharSet=utf8;port={4};";
            return string.Format(connectionStringTemplate, dataSource, db, userId, pwd, port);
        }

        /// <summary>
        /// 构建连接字符串
        /// 如 ："Database=weightsys;Data Source=192.168.88.3;User Id=admin;Password=Txmy0071;pooling=false;CharSet=utf8;port=33069";
        /// </summary>
        /// <param name="server">数据源 | IP</param>
        /// <param name="dbName">数据库存名称</param>
        /// <param name="userId">用户名称</param>
        /// <param name="pwd">密码</param>
        /// <param name="port">端口</param>
        /// <param name="polling">启用线程池</param>
        /// <returns></returns>        
        [Obsolete("已经过时，请使用 GetConnectionString 方法代替", false)]
        public static String BuildConnectionString(string server, string dbName, string userId, string pwd, int port = 3306, bool polling = true)
        {
            String connectionStringTemplate = "Server={1};Database={0};User Id={2};Password={3};pooling={4};CharSet=utf8;port={5};";
            return string.Format(connectionStringTemplate, server, dbName, userId, pwd, polling, port);
        }

        /// <summary>
        /// 构建连接字符串
        /// 如 ："Server=192.168.88.3;Database=weightsys;User Id=admin;Password=yourPwd;pooling=false;CharSet=utf8;port=33069";
        /// </summary>
        /// <param name="server">数据源 | IP</param>
        /// <param name="dbName">数据库存名称</param>
        /// <param name="userId">用户名称</param>
        /// <param name="password">密码</param>
        /// <param name="port">端口</param>
        /// <param name="polling">启用线程池</param>
        /// <returns></returns>
        public static String GetConnectionString(string server, string dbName, string userId, string password, int port = 3306, bool polling = true)
        {
            return new MySqlConnectionStringBuilder
            {
                PersistSecurityInfo = false,
                Server = server,
                Database = dbName,
                Password = password,
                UserID = userId,
                Port = (uint)port,
                Pooling = polling,
                CharacterSet = "utf8"
            }.GetConnectionString(true);
        }
    }
}
