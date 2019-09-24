using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SqlDao;
using System.Configuration;
namespace SqlDaoDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            App.MainSqlHelper.IsOpened();
         
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //TestInsert();

           // TestDelete();

           // TestSelect();

           // TestUpdate();

            InsertOrUpdate();
        }

        private void TestInsert() {
            //1
            String sql = "INSERT INTO `user` (`name`, `age`, `salary`) VALUES ( 'WolfCode', '27', '3900.90');";
            int res = App.MainSqlHelper.Insert(sql);
            if (res > 0)
            {
                Console.WriteLine("Insert successed");
            }
            else
            {
                Console.WriteLine("Insert failured");
            }
            //2 对像用法
            User user = new User
            {
                Name = "WolfCode",
                Age = 27,
                Salary = (decimal)3900.90,
            };
            int result = App.MainSqlHelper.Insert(user);

            if (result > 0)
            {
                Console.WriteLine("Insert successed");
            }
            else {
                Console.WriteLine("Insert failured");
            }
            //3
            String insertSql = SqlBuilder.GetInsertSql(user);
            int a = App.MainSqlHelper.Insert(insertSql);
            if (a > 0)
            {
                Console.WriteLine("Insert successed");
            }
            else
            {
                Console.WriteLine("Insert failured");
            }
        }
        private void TestDelete() {

            //1
            //string sql = " delete from user where id >5 ;";          
            //int res = App.MainSqlHelper.Delete(sql);
            //if(res > 0)
            //{
            //    Console.WriteLine($"成功删除 {res} 条数据");
            //}
            //else
            //{
            //    Console.WriteLine($"删除失败");
            //}
            //2
            User user = new User { Id = 7};
            int rows = App.MainSqlHelper.Delete(user);

           // int rows = App.MainSqlHelper.Delete(user,isTrueDelete:false); //不删除数据，把字段is_delete 改为 1
            if (rows > 0)
            {
                Console.WriteLine($"成功删除");
            }
            else
            {
                Console.WriteLine($"删除失败");
            }
        }
        private void TestSelect() {
            string sql = SqlBuilder.GetSelectSql(TableName.user.ToString());
            string sql1 = SqlBuilder.GetSelectSql(TableName.user.ToString(), fields: "id ,name", conditon: "id >5 and is_delete =0");               
            string sql2 = SqlBuilder.GetSelectSql(TableName.user.ToString(),fields:null,conditon:null,groupBy:null,having:null,orderBy:"id desc",limit:10,offset:0);


        }
        private void TestUpdate() {
            //1
            User user = new User
            {   Id = 5, //数据表中一定要有这条数据。否则修改失败
                Name = "Wolf123",
                Age = 27,
                Salary = (decimal)3900.90,
            };
            int result = App.MainSqlHelper.Update(user);
            if (result > 0)
            {
                Console.WriteLine("Insert successed");
            }
            else
            {
                Console.WriteLine("Insert failured");
            }
            // 2
           // string sql2 = SqlBuilder.GetUpdateSql(TableName.user.ToString(), "anme ='Wolf123' , age='27'", "id = 5 ");
            string sql = SqlBuilder.GetUpdateSql(user);

            int res = App.MainSqlHelper.Update(sql);
            //int res = App.MainSqlHelper.Update(sql2);
            if (res > 0)
            {
                Console.WriteLine("update successed");
            }
            else
            {
                Console.WriteLine("update failured");
            }

            //3 upadte or insert
           

        }

        private void InsertOrUpdate()
        {
            User user = new User
            {
                Id = 9, //数据表中有这条数据。则修改否则增加
                Name = "Wolf123",
                Age = 27,
                Salary = (decimal)3900.90,
            };
            int res = App.MainSqlHelper.InsertOrUpdate(user);
            if (res > 0)
            {
                Console.WriteLine("InsertOrUpdate successed");
            }
            else
            {
                Console.WriteLine("InsertOrUpdate failured");
            }
        }

        private void SqlBuild_Click(object sender, RoutedEventArgs e)
        {
            User user = new User();
            string selectSql = SqlBuilder.GetSelectSql(TableName.user.ToString());
            Console.WriteLine("selectSql:" + selectSql);

        }
        /// <summary>
        /// test is  connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String connstr = ConfigurationManager.ConnectionStrings["mysqlConn"].ConnectionString.ToString();
            MySqlHelper mySqlHelper=   new MySqlHelper(connstr);
            if (mySqlHelper.IsConnecting())
            {
                Console.WriteLine("----IsConnected: True");
            }
            else {
                Console.WriteLine("----IsConnected: False");
            }

            if (mySqlHelper.IsOpened())
            {
                Console.WriteLine("----IsOpened: True");
            }
            else
            {
                Console.WriteLine("----IsOpened: False");
            }
        }

        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            User user = new User
            {
                Name = "WolfCode",
                Age = 27,
                Salary = (decimal)3900.90,
                IsDelete = 1,
                DeleteTime = DateTime.Now
            };

            String connstr = ConfigurationManager.ConnectionStrings["mysqlConn"].ConnectionString.ToString();
            MySqlHelper mySqlHelper = new MySqlHelper(connstr);
           int res = mySqlHelper.Insert(user);
            Console.WriteLine("----res: "+res);
            string selectSql = SqlBuilder.GetSelectSql("user");
            List<User> datas = mySqlHelper.Select<User>(selectSql);

            Console.WriteLine("----datas.lengh: " + datas.Count);
        }

      
    }
}
