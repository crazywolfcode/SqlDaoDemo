﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace SqlDao
{
    /// <summary>
    /// SQLite Helper
    /// </summary>
    public sealed class SQLiteHelper : DbHelper
    {
        private static string getTableSchemaSql = "SELECT name as tableName FROM sqlite_master WHERE type='table' ORDER BY name; ";
        private SQLiteConnection connection;
        /// <summary>
        /// 连接对像
        /// </summary>
        public SQLiteConnection Connection
        {
            get
            {
                if (connection == null)
                {
                    connection = new SQLiteConnection(connectionString);
                }
                if (connection.State != ConnectionState.Open)
                {
                    try
                    {
                        connection.Open();
                    }
                    catch (Exception e)
                    {
                        throw new Exception("SQLite 数据打开失败！:" + e.Message);
                    }
                }
                return connection;
            }
            set { connection = value; }
        }

        /// <summary>
        /// get the save path of the database
        /// </summary>
        /// <returns></returns>
        public static string GetDbSavePath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
        }
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="connstr">连接字符串</param>
        public SQLiteHelper(string connstr)
        {
            if (string.IsNullOrEmpty(connstr))
            {
                throw new Exception("连接的字符串不能为空。");
            }
            else
            {
            if(connstr.StartsWith("DataSource=") || connstr.StartsWith("data source=")){
                connectionString = connstr;
            }else{
                connstr = "DataSource=" + connstr;
                }
            }
        }

        /// <summary>
        /// 检查连接能否打开
        /// </summary>
        /// <param name="connstring"></param>
        /// <returns></returns>
        public override bool CheckConn(string connstring)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connstring))
            {
                try
                {
                    conn.Open();
                    return conn.State == ConnectionState.Open;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 按SQl语句查询 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns>DataTable</returns>
        public override List<T> Select<T>(string sql)
        {
            DataTable dt = ExcuteDataTable(sql);

            Type type = typeof(T);
            List<T> list = new List<T>();

            foreach (DataRow row in dt.Rows)
            {
                PropertyInfo[] pArray = type.GetProperties();
                T entity = new T();
                foreach (PropertyInfo p in pArray)
                {
                    if (row[StringHelper.CamelCaseToDBnameing(p.Name)] is DBNull)
                    {
                        p.SetValue(entity, null, null);
                        continue;
                    }
                    p.SetValue(entity, row[StringHelper.CamelCaseToDBnameing(p.Name)], null);
                }
                list.Add(entity);
            }
            return list;
        }

        /// <summary>
        /// get the all tables in the databaes
        /// 获取数据中所有表
        /// </summary>
        /// <returns></returns>
        public override DataTable GetAllTable(String dbname = null)
        {
            string sql = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;  ";
            return this.ExcuteDataTable(sql);
        }
        /// <summary>
        /// 获取所有表的所有列结构
        /// </summary>
        /// <param name="dbname">库名</param>
        /// <returns></returns>
        public override List<T> GetAllTableSchema<T>(String dbname = null)
        {
            List<T> dss = new List<T>();
            DataTable dt = this.ExcuteDataTable(getTableSchemaSql);
            string json = JsonHelper.ObjectToJson(dt);
            dss = (List<T>)JsonHelper.JsonToObject(json, typeof(List<T>));
            return dss;
        }
        /// <summary>
        /// 数据表的结构语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public override string GetCreateSql(string tableName)
        {
            string sql = $"SELECT name,sql FROM sqlite_master WHERE type='table' and name = '{tableName}' ORDER BY name  ; ";
            DataTable dt = this.ExcuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][1].ToString();
            }
            return null;
        }
        /// <summary>
        /// get the schema of table
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public override List<T> GetTableSchema<T>(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                return null;
            }
            List<T> ts = null;
            string sql = string.Format(" PRAGMA table_info({0});", tableName);
            DataTable dt = this.ExcuteDataTable(sql);
            String json = JsonHelper.ObjectToJson(dt);
            ts = (List<T>)JsonHelper.JsonToObject(json, typeof(List<T>));
            return ts;
        }
        /// <summary>
        /// get the all table's name of in the database
        /// </summary>
        /// <returns></returns>
        public override string[] GetAllTableName(string dbnmae = "")
        {
            DataTable dt = GetAllTable();
            if (dt.Rows.Count <= 0) { return null; }
            string[] result = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                result[i] = dt.Rows[i]["name"].ToString();
            }
            return result;
        }
        /// <summary>
        /// 检查表是否存在
        /// </summary>
        /// <param name="db">库</param>
        /// <param name="tableName">表</param>
        /// <returns></returns>
        public override bool ExistTable(string db, string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                return false;
            }
            string sql = $"SELECT name as tableName FROM sqlite_master WHERE type='table' and name ='{tableName}'; ";
            DataTable dt = this.ExcuteDataTable(sql);
            if (dt.Rows.Count > 0) return true;
            return false;
        }

        /// <summary>  
        /// SQLite查询  
        /// </summary>   
        /// <param name="sql">要执行的sql语句</param>  
        /// <returns>结果DataTable</returns>  
        public override DataTable ExcuteDataTable(string sql)
        {
            DataTable dt = new DataTable();
            using (SQLiteCommand command = Connection.CreateCommand())
            {               
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                adapter.Fill(dt);
                return dt;
            }
        }


        private int Query(string sql, Dictionary<string, object> parametes = null)
        {
            List<SQLiteParameter> sqlite_param = new List<SQLiteParameter>();
            if (parametes != null)
            {
                foreach (KeyValuePair<string, object> row in parametes)
                {
                    sqlite_param.Add(new SQLiteParameter(row.Key, row.Value.ToString()));
                }
            }
            return this.ExecuteNonQuery(sql, sqlite_param.ToArray());
        }

        #region 增删改  

        /// <summary>  
        /// SQLite增删改  
        /// </summary>  
        /// <param name="sql">要执行的sql语句</param>  
        /// <param name="parametes">所需参数</param>  
        /// <returns>所受影响的行数</returns>  
        public int ExecuteNonQuery(string sql, SQLiteParameter[] parametes)
        {
            int affectedRows = 0;
            System.Data.Common.DbTransaction transation;
            transation = Connection.BeginTransaction();
            try
            {
                using (SQLiteCommand command = new SQLiteCommand(Connection))
                {
                    command.CommandText = sql;
                    if (parametes != null)
                    {
                        command.Parameters.AddRange(parametes);
                    }
                    affectedRows = command.ExecuteNonQuery();
                    transation.Commit();
                }
            }
            catch (Exception)
            {
                transation.Rollback();
                throw;
            }
            finally
            {
                Connection.Cancel();
            }
            return affectedRows;
        }
        /// <summary>
        /// 事务处理多条多条操作
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        public override int TransactionExecute(string[] sqls)
        {
            int affectedRows = 0;
            System.Data.Common.DbTransaction transation = Connection.BeginTransaction();
            try
            {
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Connection = Connection;
                    for (int i = 0; i < sqls.Length; i++)
                    {
                        command.CommandText = sqls[i];
                        affectedRows = command.ExecuteNonQuery();
                    }
                    transation.Commit();
                }
            }
            catch (Exception)
            {
                transation.Rollback();
                throw;
            }
            finally
            {
                Connection.Close();
            }
            return affectedRows;
        }
        /// <summary>
        /// 执行该命令并返回受影响的插入更新的行数
        /// </summary>
        /// <param name="sql"> Sql </param>
        /// <returns>行数</returns>
        public int ExcuteNoQuery(string sql)
        {
            using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
            {
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 执行删除语句
        /// </summary>
        /// <param name="sql">删除语句</param>
        /// <returns>影响行数</returns>
        public override int Delete(string sql)
        {
            return ExecuteNonQuery(sql, null);
        }
        /// <summary>
        /// 删除对像 ，支持软件删除
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="obj">对像</param>
        /// <param name="isTrueDelete">是否真的删除 默认软删除</param>
        /// <returns>影响行数</returns>
        public override int Delete<T>(T obj, Boolean isTrueDelete = false)
        {
            string deleteSql = string.Empty;
            deleteSql = SqlBuilder.GetDeleteSql(obj, isTrueDelete);
            return ExecuteNonQuery(deleteSql, null);
        }

        /// <summary>
        /// 执行修改语句
        /// </summary>
        /// <param name="sql">删除语句</param>
        /// <returns>影响行数</returns>
        public override int Update(string sql)
        {
            return ExecuteNonQuery(sql, null);
        }
        /// <summary>
        /// 修改对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="obj">对像</param>
        /// <returns>影响行数</returns>
        public override int Update<T>(T obj)
        {
            string deleteSql = string.Empty;
            deleteSql = SqlBuilder.GetUpdateSql(obj);
            return ExecuteNonQuery(deleteSql, null);
        }

        /// <summary>
        /// 执行插入语句
        /// </summary>
        /// <param name="sql">插入语句</param>
        /// <returns>影响行数</returns>
        public override int Insert(string sql)
        {
            return ExecuteNonQuery(sql, null);
        }
        /// <summary>
        /// 插入对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="obj">对像</param>
        /// <returns>影响行数</returns>
        public override int Insert<T>(T obj)
        {
            string insertSql = string.Empty;
            insertSql = SqlBuilder.GetInsertSql(obj);
            return ExecuteNonQuery(insertSql, null);
        }
        #endregion

        /// <summary>  
        /// 查询数据库所有表信息  
        /// </summary>  
        /// <returns>数据库表信息DataTable</returns>  
        public DataTable GetSchema()
        {
            return Connection.GetSchema("TABLES");
        }

        /// <summary>
        /// 插入或者更新对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="obj">对像</param>
        /// <returns>影响行数</returns>
        public override int InsertOrUpdate<T>(T obj)
        {
            if (!CheckExist(obj))
            {
                return this.Insert(obj);
            }
            else
            {
                return this.Update(obj);
            }
        }

        /// <summary>
        /// 检查对像是已经存在于数据库中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool CheckExist<T>(T obj)
        {
            Type type = typeof(T);
            try
            {
                PropertyInfo propertyinfo = type.GetProperty("id");
                if (propertyinfo == null)
                {
                    propertyinfo = type.GetProperty("Id");
                    if (propertyinfo == null)
                    {
                        throw new Exception(SqlBuilder.buildSqlErrorMessage);
                    }
                }
                object tempObj = propertyinfo.GetValue(obj, null);
                if (tempObj == null || tempObj.ToString().Length <= 0)
                {
                    throw new Exception(SqlBuilder.buildSqlErrorMessage);
                }
                string condition = SqlBuilder.splitChar + "id" + SqlBuilder.splitChar + "=" + SqlBuilder.valueSplitChar + tempObj.ToString() + SqlBuilder.valueSplitChar;
                string sql = SqlBuilder.GetSelectSql(SqlBuilder.GetTableName(obj), null, condition);
                DataTable dt = this.ExcuteDataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

        }
        /// <summary>
        /// 获取所有表的所有列结构
        /// </summary>
        /// <param name="dbname">库名</param>
        /// <param name="tablename">表名</param>
        /// <returns></returns>
        public List<MysqlTableColumnSchema> GetTableColumnSchema(string dbname, string tablename)
        {
            throw new NotImplementedException("not suport db type");
        }
        /// <summary>
        /// 是否正在连接
        /// </summary>
        /// <returns></returns>
        public override bool IsConnecting()
        {
            return Connection.State == ConnectionState.Connecting;
        }
        /// <summary>
        /// 连接是否已经打开
        /// </summary>
        /// <returns></returns>
        public override bool IsOpened()
        {
            return Connection.State == ConnectionState.Open;
        }

        /// <summary>
        /// 根据Id查找对像
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns> 对像  or null</returns>
        public override T FindById<T>(int id)
        {
            object t = Activator.CreateInstance(typeof(T));
            string tableName = StringHelper.CamelCaseToDBnameing(t.GetType().Name);
            String sql = SqlBuilder.GetSelectSql(tableName, "id = " + id);
            return Find<T>(sql);
        }
        /// <summary>
        /// 根据Sql 语句查找一个对像
        /// </summary>
        /// <typeparam name="T">要查找一个对像</typeparam>
        /// <param name="sql">Sql 语句</param>
        /// <returns></returns>
        public override T Find<T>(string sql)
        {
            DataTable dt = ExcuteDataTable(sql);
            if (dt.Rows.Count <= 0)
            {
                return null;
            }
            Type type = typeof(T);
            DataRow row = dt.Rows[0];
            PropertyInfo[] pArray = type.GetProperties();
            T entity = new T();
            foreach (PropertyInfo p in pArray)
            {
                if (row.Table.Columns.Contains(StringHelper.CamelCaseToDBnameing(p.Name)))
                {
                    if (row[StringHelper.CamelCaseToDBnameing(p.Name)] is DBNull)
                    {
                        p.SetValue(entity, null, null);
                        continue;
                    }
                    p.SetValue(entity, row[StringHelper.CamelCaseToDBnameing(p.Name)], null);
                }
            }
            return entity;
        }
    }
}
