using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using SqlDao;
namespace SqlDaoDemo
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static MySqlHelper mySqlHelper;
        public static MySqlHelper MainSqlHelper {
            get
            {
              
                if (mySqlHelper == null)
                {
                    String connstr = ConfigurationManager.ConnectionStrings["mysqlConn"].ConnectionString.ToString();
                    mySqlHelper = new MySqlHelper(connstr);
                }

                return mySqlHelper;
            }
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
         
        }

    }
}
