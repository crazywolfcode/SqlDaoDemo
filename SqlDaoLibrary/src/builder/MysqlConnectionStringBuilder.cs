using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDao
{
  public  class MysqlConnectionStringBuilder :SqlBuilder
    {
        /// <summary>
        /// 构建连接字符串
        /// 如 ："Database=weightsys;Data Source=192.168.88.3;User Id=admin;Password=Txmy0071;pooling=false;CharSet=utf8;port=33069";
        /// </summary>
        /// <param name="dataSource">数据源 | IP</param>
        /// <param name="db">数据库存名称</param>
        /// <param name="userId">用户名称</param>
        /// <param name="pwd">密码</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        public static String BuildConnectionString(string dataSource, string db, string userId, string pwd, string port)
        {         
            String connectionStringTemplate = "Database={0};Data Source={1};User Id={2};Password={3};pooling=false;CharSet=utf8;port={4};";
            return string.Format(connectionStringTemplate, db, dataSource, userId, pwd, port);
        }
    }
}
