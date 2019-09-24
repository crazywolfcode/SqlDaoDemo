using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlDao
{

    public class MySqlHelper : DbHelper
    {
        public MySqlConnection connection;
        // 
        //异常:
        //
        //   T:System.InvalidOperationException:
        //     Cannot open a connection without specifying a data source or server.
        //
        //   T:MySql.Data.MySqlClient.MySqlException:
        //     A connection-level error occurred while opening the connection.
        //
        public MySqlConnection Connection
        {
            get
            {
                if (connection == null)
                {
                    connection = new MySqlConnection(connectionString);
                    if (connection.State != ConnectionState.Open)
                    {
                        try
                        {
                            connection.Open();
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Mysql 数据打开失败！:" + e.Message);
                        }
                    }
                    return connection;
                }
                else
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        try
                        {
                            connection.Open();
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Mysql 数据打开失败！:" + e.Message);
                        }
                    }
                    return connection;
                }
            }

        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public MySqlHelper(string connstring)
        {
            if (!string.IsNullOrEmpty(connstring))
            {
                connectionString = connstring;
            }
            if (connectionString == null || connectionString.Length <= 0)
            {
                throw new Exception("Mysql 数据没有正常的配制！");
            }
        }
        /// <summary>
        /// 检查连接能否打开
        /// </summary>
        /// <param name="connstring"></param>
        /// <returns></returns>
        public override bool CheckConn(string connstring = null)
        {
            if (String.IsNullOrEmpty(connstring))
            {
                connstring = connectionString;
            }
            using (MySqlConnection conn = new MySqlConnection(connstring))
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
        /// get the all tables in the databaes
        /// 获取数据中所有表
        /// </summary>
        /// <returns></returns>
        public override DataTable GetAllTable(string dbbame)
        {
            string sql = $"SELECT table_name as `name` from information_schema.tables where table_schema='{dbbame}' and table_type='base table';";
            return this.ExcuteDataTable(sql, null);
        }
        /// <summary>
        /// get the all table's name of in the database
        /// </summary>
        /// <returns></returns>
        public override string[] GetAllTableName(string dbbame)
        {
            DataTable dt = GetAllTable(dbbame);
            if (dt.Rows.Count <= 0) { return null; }
            string[] result = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                result[i] = dt.Rows[i]["name"].ToString();
            }
            return result;
        }

        public override List<DbSchema> GetAllTableSchema(string dbname)
        {
            List<DbSchema> dss = new List<DbSchema>();
            string sql = $"SELECT TABLE_NAME as tableName,TABLE_COMMENT as tableComment,CREATE_TIME as createTime,UPDATE_TIME as updateTime ,TABLE_ROWS as tableRows,DATA_LENGTH as dataLength   from information_schema.tables where table_schema='{dbname}'  and table_type='base table';";
            DataTable dt = this.ExcuteDataTable(sql, null);
            string json = JsonHelper.ObjectToJson(dt);
            dss = (List<DbSchema>)JsonHelper.JsonToObject(json, typeof(List<DbSchema>));
            return dss;
        }
        /// <summary>
        /// mysql 8.0 
        /// </summary>
        /// <param name="dbname"></param>
        /// <returns></returns>
        public List<DbSchema> getAllTableSchema8(string dbname)
        {
            List<DbSchema> dss = new List<DbSchema>();
            string sql = $"SELECT TABLE_NAME as tableName,TABLE_COMMENT as tableComment,CREATE_TIME as createTime,UPDATE_TIME as updateTime ,TABLE_ROWS as tableRows,DATA_LENGTH as dataLength   from information_schema.tables where table_schema='{dbname}'  and (table_type='base table' or table_type='BASE TABLE');";
            DataTable dt = this.ExcuteDataTable(sql, null);
            string json = JsonHelper.ObjectToJson(dt);
            dss = (List<DbSchema>)JsonHelper.JsonToObject(json, typeof(List<DbSchema>));
            return dss;
        }
        /// <summary>
        /// judge the table is or not exist
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public override bool ExistTable(string dbName, string table)
        {
            if (string.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(table))
            {
                return false;
            }
            string sql = $"SELECT * FROM information_schema.TABLES WHERE TABLE_SCHEMA = '{dbName}' and table_name ='{table}' and table_type='base table' ;";
            DataTable dt = this.ExcuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
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
            string sql = string.Format(" desc {0};", tableName);
            DataTable dt = this.ExcuteDataTable(sql, null);
            String json = JsonHelper.ObjectToJson(dt);
            ts = (List<TableScema>)JsonHelper.JsonToObject(json, typeof(List<MysqlTabeSchema>));
            return ts;
        }

        public List<MysqlTableColumnSchema> GetTableColumnSchema(string dbname, string tablename)
        {
            List<MysqlTableColumnSchema> list = null;
            if (string.IsNullOrEmpty(dbname) && string.IsNullOrEmpty(tablename))
            {
                return null;
            }
            string sqlT = @"select COLUMN_NAME as columnName ,
                                   COLUMN_TYPE as type,
			                       COLUMN_DEFAULT as defaultValue,
			                       IS_NULLABLE as isNullable,
			                       COLUMN_COMMENT as commentValue
                            from information_schema.columns 
                            where table_schema ='{0}'  and table_name = '{1}';";
            string sql = string.Format(sqlT, dbname, tablename);
            DataTable dt = this.ExcuteDataTable(sql, null);
            String json = JsonHelper.ObjectToJson(dt);
            list = (List<MysqlTableColumnSchema>)JsonHelper.JsonToObject(json, typeof(List<MysqlTableColumnSchema>));
            return list;
        }

        public override string GetCreateSql(string tableName)
        {
            string sql = $"show create table {tableName};";
            DataTable dt = this.ExcuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][1].ToString();
            }
            return null;
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
                    if (row.Table.Columns.Contains(StringHelper.camelCaseToDBnameing(p.Name)))
                    {
                        if (row[StringHelper.camelCaseToDBnameing(p.Name)] is DBNull)
                        {
                            p.SetValue(entity, null, null);
                            continue;
                        }
                        p.SetValue(entity, row[StringHelper.camelCaseToDBnameing(p.Name)], null);
                    }
                }
                list.Add(entity);
            }
            return list;
        }

        /// <summary>  
        /// 查询  
        /// </summary>  
        /// <param name="sql">要执行的sql语句</param>  
        /// <param name="parameters">所需参数</param>  
        /// <returns>结果DataTable</returns>  
        public override DataTable ExcuteDataTable(string sql, MySqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            using (MySqlCommand command = new MySqlCommand(sql, Connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    adapter.Fill(dt);
                }
                return dt;
            }
        }
        /// <summary>  
        /// 查询  
        /// </summary>  
        /// <param name="sql">要执行的sql语句</param>  
        /// <param name="parameters">所需参数</param>  
        /// <returns>结果DataTable</returns>  
        public override DataTable ExcuteDataTable(string sql)
        {
            DataTable dt = new DataTable();
            using (MySqlCommand command = new MySqlCommand(sql, Connection))
            {
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    adapter.Fill(dt);
                }
                return dt;
            }
        }
    
        #region 增删改  

        /// <summary>  
        /// 增删改  
        /// </summary>  
        /// <param name="sql">要执行的sql语句</param>  
        /// <param name="parameters">所需参数</param>  
        /// <returns>所受影响的行数</returns>  
        public int ExecuteNonQuery(string sql, MySqlParameter[] parametes)
        {
            int affectedRows = 0;
            DbTransaction transation = Connection.BeginTransaction();            
            try
            {
                using (MySqlCommand command = new MySqlCommand(sql, Connection))
                {
                    if (parametes != null)
                    {
                        command.Parameters.AddRange(parametes);
                    }
                    affectedRows = command.ExecuteNonQuery();
                    transation.Commit();
                }
            }
            catch (Exception e)
            {
                transation.Rollback();
                throw e;
            }
            finally
            {               
                Connection.Close();
            }
            return affectedRows;
        }
        /// <summary>
        /// 事务处理多条多条操作
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        public override int TransactionExecute(params string[] sqls)
        {
            int affectedRows = 0;
            DbTransaction transation = Connection.BeginTransaction();
            try
            {
                using (MySqlCommand command = new MySqlCommand()) {
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
            }
            finally
            {              
                Connection.Close();
            }
            return affectedRows;
        }

        
        /// <summary>
        /// 执行删除语句
        /// </summary>
        /// <param name="sql">删除语句</param>
        /// <returns>影响行数</returns>
        public override int Delete(string sql)
        {
            return ExecuteNonQuery(sql,null);
        }
        /// <summary>
        /// 删除对像 ，支持软件删除
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="obj">对像</param>
        /// <param name="isTrueDelete">是否真的删除 默认软删除</param>
        /// <returns>影响行数</returns>
        public override int Delete<T>(T obj, Boolean isTrueDelete = true)
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
            string sql = SqlBuilder.GetUpdateSql(obj);
            return ExecuteNonQuery(sql, null);
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
            string insertSql = SqlBuilder.GetInsertSql(obj);
            return ExecuteNonQuery(insertSql, null);
        }

        /// <summary>
        /// 插入或者更新对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="obj">对像</param>
        /// <returns>影响行数</returns>
        public override int InsertOrUpdate<T>(T obj)
        {
            if (CheckExist(obj))
            {
                return this.Update(obj);
            }
            else
            {
                return this.Insert(obj);
            }
        }

        public void GetSchema()
        {
            foreach (var item in Connection.GetSchema().Rows)
            {
                 Console.WriteLine(item.ToString());
            }

        }
        #endregion

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
                throw;
            }
            //put code in up 
        }

        public override bool IsConnecting()
        {
            return Connection.State == ConnectionState.Connecting;
        }
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
            string tableName = StringHelper.camelCaseToDBnameing(t.GetType().Name);
            String sql = SqlBuilder.GetSelectSql(tableName, null,"id = " + id);          
            return Find<T>(sql); 
        }


        public override T Find<T>(string sql)
        {
            DataTable dt = ExcuteDataTable(sql);
            if (dt.Rows.Count <= 0) {
                return (T)Null();
            }
            Type type = typeof(T);
            DataRow row = dt.Rows[0];
                PropertyInfo[] pArray = type.GetProperties();
                T entity = new T();
                foreach (PropertyInfo p in pArray)
                {
                    if (row.Table.Columns.Contains(StringHelper.camelCaseToDBnameing(p.Name)))
                    {
                        if (row[StringHelper.camelCaseToDBnameing(p.Name)] is DBNull)
                        {
                            p.SetValue(entity, null, null);
                            continue;
                        }
                        p.SetValue(entity, row[StringHelper.camelCaseToDBnameing(p.Name)], null);
                    }
                }
            return entity;
        }
    }
}
