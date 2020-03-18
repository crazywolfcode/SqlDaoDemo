using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlDao
{/// <summary>
/// 数据库存的帮助类
/// </summary>
    public abstract class DbHelper
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string connectionString = string.Empty;
        /// <summary>
        /// 是否正在连接
        /// </summary>
        /// <returns></returns>
        public abstract Boolean IsConnecting();
        /// <summary>
        /// 连接是否已经打开
        /// </summary>
        /// <returns></returns>
        public abstract Boolean IsOpened();
        /// <summary>
        /// 根据Id查找对像
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns> 对像  or null</returns>
        public abstract T FindById<T>(int id) where T : class , new();
 
        /// <summary>
        /// 按sql语句查询
        /// </summary>
        /// <param name="sql">sql语句查询</param>
        /// <returns>List</returns>
        public abstract List<T> Select<T>(string sql) where T : class, new();

        /// <summary>
        /// 按sql语句查询 一条数据
        /// </summary>
        /// <param name="sql">sql语句查询</param>
        /// <returns>T</returns>
        public abstract T Find<T>(string sql) where T : class, new();
        /// <summary>
        /// 检查对像是已经存在于数据库中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract bool CheckExist<T>(T obj);
        /// <summary>
        /// 执行 sql 语句，返回 DataTable
        /// </summary>
        /// <param name="sql"> sql 语句</param>
        /// <returns> DataTable </returns>
        public abstract DataTable ExcuteDataTable(string sql);
        /// <summary>
        /// 检查连接能否打开
        /// </summary>
        /// <param name="connstring">连接字符串</param>
        /// <returns>true  or false</returns>
        public abstract bool CheckConn(string connstring);

        /// <summary>
        /// judge the table is or not exist 表是否存在
        /// </summary>
        /// <param name="dbName">库名</param>
        /// <param name="table">表名</param>
        /// <returns></returns>
        public abstract bool ExistTable(string dbName, string table);

        /// <summary>
        /// get the all tables in the databaes
        /// 获取数据中所有表
        /// </summary>
        /// <returns></returns>
        public abstract DataTable GetAllTable(string dbbame);


        /// <summary>
        /// get the all table's name of in the database
        /// </summary>
        /// <returns></returns>
        public abstract string[] GetAllTableName(string dbname);

        /// <summary>
        /// 获取数据库中所有表的结构
        /// </summary>
        /// <typeparam name="T"> TableScema和子类</typeparam>
        /// <param name="dbname">库名</param>
        /// <returns></returns>
        public abstract List<T> GetAllTableSchema<T>(string dbname);

        /// <summary>
        /// get the schema of table
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>    
        public abstract string GetCreateSql(string tableName);

        /// <summary>
        /// 删除对像 ，支持软件删除
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="obj">对像</param>
        /// <param name="isTrueDelete">是否真的删除 默认软删除</param>
        /// <returns>影响行数</returns>
        public abstract int Delete<T>(T obj, Boolean isTrueDelete = false);
        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public abstract int Delete(string sql);
        /// <summary>
        /// 执行修改语句
        /// </summary>
        /// <param name="sql">删除语句</param>
        /// <returns>影响行数</returns>
        public abstract int Update(string sql);

        /// <summary>
        /// 修改对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="obj">对像</param>
        /// <returns>影响行数</returns>
        public abstract int Update<T>(T obj);


        /// <summary>
        /// 执行插入语句
        /// </summary>
        /// <param name="sql">插入语句</param>
        /// <returns>影响行数</returns>
        public abstract int Insert(string sql);

        /// <summary>
        /// 插入对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="obj">对像</param>
        /// <returns>影响行数</returns>
        public abstract int Insert<T>(T obj);

        /// <summary>
        /// 插入或者更新对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="obj">对像</param>
        /// <returns>影响行数</returns>
        public abstract int InsertOrUpdate<T>(T obj);

        /// <summary>
        /// 事务处理多条多条操作
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        public abstract int TransactionExecute(string[] sqls);

        /// <summary>
        /// 获取表结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public abstract List<T> GetTableSchema<T>(string tableName) where T : TableScema;
       
        /// <summary>
        /// return null 
        /// </summary>
        /// <returns></returns>
        private object Null()
        {
            return null;
        }
    }
}
