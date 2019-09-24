![logo](https://raw.githubusercontent.com/crazywolfcode/SqlDaoDemo/master/SqlDaoDemo/logo.png)
![dotnet-version](https://img.shields.io/badge/.net-%3E%3D4.0-blue.svg) 
![csharp-version](https://img.shields.io/badge/C%23-7.3-blue.svg) 
![IDE-version](https://img.shields.io/badge/IDE-vs2019-blue.svg)
[![nuget-version](https://img.shields.io/nuget/v/1.0.2.svg)](https://www.nuget.org/packages/SqlDao/1.0.1)
![qq-group](https://img.shields.io/badge/qq-443055589-red.svg)
# SqlDaoDemo
C# 操作数据库的DAO类库,CURD 操作不需要拼写SQl语句，节约时间，提高开发效率，亲测  Mysql 、SQLite 好用。

## 获取

  在nuget上添加对 SqlDao 的引用或搜索 SqlDao;

 ```Install-Package  SqlDao```
 
 ## 配制
 
    以Wpf 桌面项目为例，通常我们是把数据库的连接字符串放到 app.config 文件中
    
    ```xml
    <?xml version="1.0" encoding="utf-8" ?>
    <configuration>
      <connectionStrings>
        <add name="mysqlConn" connectionString="Database=test;Data Source=127.0.0.1;User Id=admin;Password=code@8888;pooling=false;CharSet=utf8;port=3306"/>
      </connectionStrings>
        <startup> 
            <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.7.2" />
        </startup>
    </configuration>
    ```
    
    在代码内获取配制
    
    ```
      String connstr = ConfigurationManager.ConnectionStrings["mysqlConn"].ConnectionString.ToString();
    ```
    
 ## 使用
 
 ### 最基本的使用（不推荐）
 
```
  //增加一个用户
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
```
  
  第一次操作都会 重新连接数据库，所以不推荐。
  
### 推荐使用方式

    在App.xaml.cs 中生成一个静态的变量，作为主要操作数据库的助手类。如果有多数据源，其它数据源的操作使用上面基本操作方式。
    如果不是WPF项目那就找一个全局能访问的类中初始化即可。
    
    ```
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
    ```
#### 新增 用例
   
   * 用例 1 
   ```
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
   ```
   
   * 用例2 
   
   ```
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
   ```
   
  * 用例3
  ```
   User user = new User
    {
        Name = "WolfCode",
        Age = 27,
        Salary = (decimal)3900.90,
    };
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
  ```
  
#### 更新 用例
   * 用例 1 
   ```
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
        Console.WriteLine("Update successed");
    }
    else
    {
        Console.WriteLine("Update failured");
    }
   
   ```
   * 用例 2 
   ```
   User user = new User
    {
        Id = 5, //数据表中一定要有这条数据。否则修改失败
        Name = "Wolf123",
        Age = 27,
        Salary = (decimal)3900.90,
    };
   string sql = SqlBuilder.GetUpdateSql(user); 

    int res = App.MainSqlHelper.Update(sql);
  
    if (res > 0)
    {
        Console.WriteLine("update successed");
    }
    else
    {
        Console.WriteLine("update failured");
    }
   ```
  
   * 用例 3 
   ```
     string sql2 = SqlBuilder.GetUpdateSql(TableName.user.ToString(), "anme ='Wolf123' ,
     int res = App.MainSqlHelper.Update(sql2);
     if (res > 0)
    {
        Console.WriteLine("update successed");
    }
    else
    {
        Console.WriteLine("update failured");
    }
   ```
   
#### 删除 用例

 * 用例 1 
 ```
  string sql = " delete from user where id >5 ;";          
  int res = App.MainSqlHelper.Delete(sql);
  if(res > 0)
  {
      Console.WriteLine($"成功删除 {res} 条数据");
  }
  else
  {
      Console.WriteLine($"删除失败");
  }
 ```
  
 * 用例 2
 ```
  User user = new User { Id = 7 };
  int rows = App.MainSqlHelper.Delete(user);

  // int rows = App.MainSqlHelper.Delete(user,isTrueDelete:false); //isTrueDelete:false不删除数据，把字段is_delete 改为 1,默认为true
  if (rows > 0)
  {
      Console.WriteLine($"成功删除");
  }
  else
  {
      Console.WriteLine($"删除失败");
  }
 ```

#### 查询 用例
  * 用例 1
  ```
  //查询 User表中的所有记录
  string sql = SqlBuilder.GetSelectSql(TableName.user.ToString());
  List<User> users = App.MainSqlHelper.Select<User>(sql);
  Console.WriteLine("--datas : " + users.Count);
  ```
  
  * 用例 2
   ```
  //查询 User表中 di > 5 并且 is_delete =0 的所有记录的 id 和 name 字段
    string sql1 = SqlBuilder.GetSelectSql(TableName.user.ToString(), fields: "id ,name", conditon: "id >5 and is_delete =0");
    List<User> users1 = App.MainSqlHelper.Select<User>(sql1);
    Console.WriteLine("--datas : " + users1.Count);
  ```
  
  * 用例 3
   ```
  //查询User表中的 10 条数据，按id 倒序排序
  string sql2 = SqlBuilder.GetSelectSql(TableName.user.ToString(), fields: null, conditon: null, groupBy: null, having: null, orderBy: "id desc", limit: 10, offset: 0);
  List<User> users2 = App.MainSqlHelper.Select<User>(sql2);
  Console.WriteLine("--datas : " + users2.Count);
  ```
  
  * 用例 4 多表查询需要手动拼写Sql语句
   ```
  //多表查询需要手动拼写Sql语句
  String joinSql = "SELECT u.* ,r.money,r.remark FROM record as r JOIN `user` as u where u.is_delete = 0 and u.id = r.user_id";
  List<Object> os = App.MainSqlHelper.Select<Object>(joinSql);
  ```
  
  *其它的查询类似，依照操作
  
  #### 查询或者更新 用例
  
  ```
    User user = new User
    {
        Id = 9, //数据表中有这条数据则修改否则增加
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
  
  ```
 
  #### 事务操作 用例
  
  ```
  //模拟发工资的操作，既要改变账户的金额，又要记录流水，需要用到事务。
  
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
  ```  
  
## 附加测试时使用的数据表

```
CREATE TABLE `user` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(50) NOT NULL,
  `age` int(2) NOT NULL,
  `salary` decimal(8,2) NOT NULL DEFAULT '0.00',
  `is_delete` int(1) NOT NULL DEFAULT '0',
  `delete_time` datetime DEFAULT NULL COMMENT '删除时间',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8

CREATE TABLE `account` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `user_id` int(10) NOT NULL,
  `money` decimal(8,2) NOT NULL DEFAULT '0.00' COMMENT '金额',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8

CREATE TABLE `record` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `user_id` int(10) NOT NULL,
  `money` decimal(8,2) NOT NULL DEFAULT '0.00',
  `update_time` datetime DEFAULT NULL,
  `remark` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8
```
## 附加测试时使用的数据表对应的实体类

```
public class User
    {
        public Int32 Id { get; set; }
        public String Name { get; set; }
        public Int32? Age { get; set; }
        public decimal? Salary { get; set; }
        public Int32? IsDelete { get; set; }
        public DateTime? DeleteTime { get; set; }
    }
    
     public class Account
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal? Money { get; set; }
    }
    
    public class Record
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public decimal? Money { get; set; }
        public string Remark { get; set; }
    }
```

## 建议

  在正试的项目开发过程中，对数据库的操作放在对应的 Model 中，代码更加可读。如 对User 的所有操作应放在UserModel 类中。 
  
  
  
  ## 捐赠
如果您觉得还不错，并且刚好有些闲钱，那么可以选择以下方式来捐赠：

* 支付宝  
![qrcode](https://raw.githubusercontent.com/crazywolfcode/httpHelper/master/zfb.jpg)
* 微信  
![qrcode](https://raw.githubusercontent.com/crazywolfcode/httpHelper/master/wxRevard.png)
