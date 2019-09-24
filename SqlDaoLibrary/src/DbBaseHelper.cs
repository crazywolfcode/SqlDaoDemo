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
{
    public abstract class DbHelper
    {
        public string connectionString = string.Empty;

        public abstract Boolean IsConnecting();
        public abstract Boolean IsOpened();

        /// <summary>
        /// 按sql语句查询
        /// </summary>
        /// <param name="sql">sql语句查询</param>
        /// <returns>Datatable</returns>
        public abstract List<T> Select<T>(string sql) where T : class, new();

        /// <summary>
        /// 检查对像是已经存在于数据库中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract bool CheckExist<T>(T obj);



        public abstract DataTable ExcuteDataTable(string sql, MySqlParameter[] parameters);
        public abstract DataTable ExcuteDataTable(string sql);
        /// <summary>
        /// 检查连接能否打开
        /// </summary>
        /// <param name="connstring"></param>
        /// <returns></returns>
        public abstract bool CheckConn(string connstring);

        /// <summary>
        /// judge the table is or not exist
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="table"></param>
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


        public abstract List<DbSchema> GetAllTableSchema(string dbname);

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


        public abstract List<TableScema> GetTableSchema(string tableName);
    }
}
