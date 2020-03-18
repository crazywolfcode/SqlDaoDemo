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

            //TestSelect();

            // TestUpdate();

            // InsertOrUpdate();

            TestTransation();

            TestExist();
        }

        private void TestInsert()
        {
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
            else
            {
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
        private void TestDelete()
        {

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
            User user = new User { Id = 7 };
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
        private void TestSelect()
        {
            //查询 User表中的所有记录
            string sql = SqlBuilder.GetSelectSql(TableName.user.ToString());
            List<User> users = App.MainSqlHelper.Select<User>(sql);
            Console.WriteLine("--datas : " + users.Count);
            //查询 User表中 di > 5 并且 is_delete =0 的所有记录的 id 和 name 字段
            string sql1 = SqlBuilder.GetSelectSql(TableName.user.ToString(), fields: "id ,name", conditon: "id >5 and is_delete =0");
            List<User> users1 = App.MainSqlHelper.Select<User>(sql1);
            Console.WriteLine("--datas : " + users1.Count);
            //查询User表中的 10 条数据，按id 倒序排序
            string sql2 = SqlBuilder.GetSelectSql(TableName.user.ToString(), fields: null, conditon: null, groupBy: null, having: null, orderBy: "id desc", limit: 10, offset: 0);
            List<User> users2 = App.MainSqlHelper.Select<User>(sql2);
            Console.WriteLine("--datas : " + users2.Count);

            //多表查询需要手动拼写Sql语句
            String joinSql = "SELECT u.* ,r.money,r.remark FROM record as r JOIN `user` as u where u.is_delete = 0 and u.id = r.user_id";
            List<Object> os = App.MainSqlHelper.Select<Object>(joinSql);

        }
        private void TestUpdate()
        {
            //1
            User user = new User
            {
                Id = 5, //数据表中一定要有这条数据。否则修改失败
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

        private void TestTransation()
        {
            int userId = 1;
            User user = App.MainSqlHelper.FindById<User>(userId);
            if (user != null)
            {
                String asql = SqlBuilder.GetSelectSql(TableName.account.ToString(), null, "user_id = " + user.Id);
                Account account = App.MainSqlHelper.Find<Account>(asql);
                string accSql;
                if (account != null)
                {
                    account.Money += user.Salary;
                    accSql = SqlBuilder.GetUpdateSql(account);
                }
                else
                {
                    account = new Account
                    {
                        UserId = user.Id,
                        Money = user.Salary
                    };
                    accSql = SqlBuilder.GetInsertSql(account);
                }
                Record record = new Record
                {
                    Id = 1,
                    UserId = user.Id,
                    Money = user.Salary,
                    UpdateTime = DateTime.Now,
                    Remark = "发11 月份工资"
                };
                string insertsql = SqlBuilder.GetInsertSql(record);
                string[] sqls = new string[] { accSql, insertsql };

                // statr transation
                int res = App.MainSqlHelper.TransactionExecute(sqls);

                if (res > 0)
                {
                    Console.WriteLine("操作成功！");
                }
                else
                {
                    Console.WriteLine("操作失败！");
                }
            }
        }

        private void TestExist()
        {
            //判断用户id 为1 的用户是否存在
            User user = new User { Id = 1, };

            Boolean res = App.MainSqlHelper.CheckExist<User>(user);
            if (res)
            {
                // 存在
            }
            else
            {
                //不存在
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
            MySqlHelper mySqlHelper = new MySqlHelper(connstr);
            if (mySqlHelper.IsConnecting())
            {
                Console.WriteLine("----IsConnected: True");
            }
            else
            {
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
            Console.WriteLine("----res: " + res);
            string selectSql = SqlBuilder.GetSelectSql("user");
            List<User> datas = mySqlHelper.Select<User>(selectSql);

            Console.WriteLine("----datas.lengh: " + datas.Count);
        }


    }
}
