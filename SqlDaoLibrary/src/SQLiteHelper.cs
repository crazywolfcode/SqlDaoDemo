using System;
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

    public sealed class SQLiteHelper : DbHelper
    {
        public static string getTableSchemaSql = "SELECT name as tableName FROM sqlite_master WHERE type='table' ORDER BY name; ";
        public SQLiteConnection connection;

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

        public SQLiteHelper(string connstr)
        {
            if (string.IsNullOrEmpty(connstr))
            {
                throw new Exception("连接的字符串不正确。");
            }
            else
            {
                if (!connstr.StartsWith("DataSource="))
                    connstr = "DataSource=" + connstr;
                connectionString = connstr;
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
                    if (row[StringHelper.camelCaseToDBnameing(p.Name)] is DBNull)
                    {
                        p.SetValue(entity, null, null);
                        continue;
                    }
                    p.SetValue(entity, row[StringHelper.camelCaseToDBnameing(p.Name)], null);
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
            return this.ExcuteDataTable(sql, null);
        }

        public override List<DbSchema> GetAllTableSchema(String dbname = null)
        {
            List<DbSchema> dss = new List<DbSchema>();
            DataTable dt = this.ExcuteDataTable(getTableSchemaSql, null);
            string json = JsonHelper.ObjectToJson(dt);
            dss = (List<DbSchema>)JsonHelper.JsonToObject(json, typeof(List<DbSchema>));
            return dss;
        }

        public override string GetCreateSql(string tablename)
        {
            string sql = $"SELECT name,sql FROM sqlite_master WHERE type='table' and name = '{tablename}' ORDER BY name  ; ";
            DataTable dt = this.ExcuteDataTable(sql, null);
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
        public override List<TableScema> GetTableSchema(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                return null;
            }
            List<TableScema> ts = null;
            string sql = string.Format(" PRAGMA table_info({0});", tableName);
            DataTable dt = this.ExcuteDataTable(sql, null);
            String json = JsonHelper.ObjectToJson(dt);
            ts = (List<TableScema>)JsonHelper.JsonToObject(json, typeof(List<SqliteTableSchema>));
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
        /// 获取多行  
        /// </summary>  
        /// <param name="sql">执行sql</param>  
        /// <param name="param">sql参数</param>  
        /// <returns>多行结果</returns>  
        public DataRow[] getRows(string sql, Dictionary<string, object> param = null)
        {
            List<SQLiteParameter> sqlite_param = new List<SQLiteParameter>();

            if (param != null)
            {
                foreach (KeyValuePair<string, object> row in param)
                {
                    sqlite_param.Add(new SQLiteParameter(row.Key, row.Value.ToString()));
                }
            }
            DataTable dt = this.ExcuteDataTable(sql, sqlite_param.ToArray());
            return dt.Select();
        }

        public override bool ExistTable(string db, string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                return false;
            }
            string sql = $"SELECT name as tableName FROM sqlite_master WHERE type='table' and name ='{tableName}'; ";
            DataTable dt = this.ExcuteDataTable(sql, null);
            if (dt.Rows.Count > 0) return true;
            return false;
        }

        /// <summary>  
        /// SQLite查询  
        /// </summary>   
        /// <param name="sql">要执行的sql语句</param>  
        /// <param name="parameters">所需参数</param>  
        /// <returns>结果DataTable</returns>  
        public DataTable ExcuteDataTable(string sql, SQLiteParameter[] parameters = null)
        {
            DataTable dt = new DataTable();
            using (SQLiteCommand command = Connection.CreateCommand())
            {
                command.CommandText = sql;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                adapter.Fill(dt);
                return dt;
            }
        }


        public int query(string sql, Dictionary<string, object> parametes = null)
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
        /// <param name="parameters">所需参数</param>  
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

        public List<MysqlTableColumnSchema> GetTableColumnSchema(string dbname, string tablename)
        {
            throw new NotImplementedException("not suport db type");
        }

        public override DataTable ExcuteDataTable(string sql, MySqlParameter[] parameters)
        {
            return ExcuteDataTable(sql);
        }
        public override DataTable ExcuteDataTable(string sql)
        {
            return ExcuteDataTable(sql);
        }

        public override bool IsConnecting()
        {
            return Connection.State == ConnectionState.Connecting;
        }

        public override bool IsOpened()
        {
            return Connection.State == ConnectionState.Open;
        }
    }
}
