using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SqlDao
{
    /// <summary>
    /// sql 语句 构建器
    /// </summary>
    public class SqlBuilder
    {
        /// <summary>
        /// , 号
        /// </summary>
        public static readonly char splitChar = '`';
        /// <summary>
        /// '
        /// </summary>
        public static readonly string valueSplitChar = "'";
        private static readonly string notSoftDeleteWhere = splitChar + softDeleteColumnName + splitChar + "=" + valueSplitChar + nomalStatusTag + valueSplitChar;
        private static readonly string selectSqlTemplate = "SELECT {0} FROM {1} WHERE {2} ;";
        private static readonly string groupByTemplate = " GROUP BY {0} ";
        private static readonly string orderByTemplate = " ORDER BY {0} ";
        private static readonly string havingTemplate = " HAVING  {0} ";
        private static readonly string LimitTemplate = " LIMIT  {0} ";
        private static readonly string offsetTemplate = " OFFSET  {0} ";

        private static readonly string insertSqlTemplate = "INSERT INTO {0} ({1}) VALUES ({2});";
        private static readonly string updateSqlTemplate = "UPDATE {0} SET {1} WHERE {2};";
        private static readonly string deleteSqlTemplate = "DELETE FROM {0} WHERE {1};";

        //软删除的数据字段名
        private static readonly string softDeleteColumnName = "is_delete";
        //软删除的实体类属性名
        private static readonly string softDeletePropertyName = "IsDelete";
        private static readonly string deleteStatusTag = "1"; //删除状态标识
        private static readonly string nomalStatusTag = "0";//正常状态标识
        //软删除的条件
        private static readonly string softDeleteWhere = splitChar + softDeleteColumnName + splitChar + "=" + valueSplitChar + deleteStatusTag + valueSplitChar;
        private static readonly string softDeleteSet = splitChar + softDeleteColumnName + splitChar + "=" + valueSplitChar + deleteStatusTag + valueSplitChar;
        /// <summary>
        /// 错误提示
        /// </summary>
        public static readonly string buildSqlErrorMessage = "无法获取id或Id的属性名或值，无法对生成SQL的Where条件！";

        /// <summary>
        /// 获取一个对像所对应的数据表名
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">类型的对像</param>
        /// <returns>数据表名</returns>
        public static string GetTableName<T>(T obj)
        {
            Type type = typeof(T);
            string name = StringHelper.CamelCaseToDBnameing(type.Name);
            return name;
        }

        /// <summary>
        /// 查询SQL语句 自动加上软删除的条件
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fields"></param>
        /// <param name="conditon"></param>
        /// <param name="groupBy"></param>
        /// <param name="having"></param>
        /// <param name="orderBy"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public string GetSelectSqlWithSoftDeleteCondition(string tableName, string fields = null, string conditon = null, string groupBy = null, string having = null, string orderBy = null, int limit = 0, int offset = -1)
        {
            string sql = string.Empty;
            if (string.IsNullOrEmpty(fields))
            {
                fields = " * ";
            }
            if (string.IsNullOrEmpty(conditon))
            {
                conditon = notSoftDeleteWhere;
            }
            else
            {
                conditon = "(" + conditon + ")" + " and " + notSoftDeleteWhere;
            }
            sql = string.Format(selectSqlTemplate, fields, tableName, conditon);

            if (!string.IsNullOrEmpty(groupBy))
            {
                sql = sql.Replace(";", string.Format(groupByTemplate, groupBy) + " ;");
            }

            if (!string.IsNullOrEmpty(having))
            {
                sql = sql.Replace(";", string.Format(havingTemplate, having) + " ;");
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                sql = sql.Replace(";", string.Format(orderByTemplate, orderBy) + " ;");
            }
            if (limit > 0)
            {
                sql = sql.Replace(";", string.Format(LimitTemplate, limit) + " ;");
            }

            if (offset > -1)
            {
                sql = sql.Replace(";", string.Format(offsetTemplate, offset) + " ;");
            }
            return sql;
        }

        /// <summary>
        /// 获得查询SQL语句 去除软删除的条件
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="fields">字段</param>
        /// <param name="conditon">条件</param>
        /// <param name="groupBy">分组</param>
        /// <param name="having">分组条件</param>
        /// <param name="orderBy">排序</param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static string GetSelectSql(string tableName, string fields = null, string conditon = null, string groupBy = null, string having = null, string orderBy = null, int limit = 0, int offset = -1)
        {
            string sql = string.Empty;
            if (string.IsNullOrEmpty(fields))
            {
                fields = " * ";
            }
            sql = string.Format(selectSqlTemplate, fields, tableName, conditon);
            if (String.IsNullOrEmpty(conditon))
            {
                sql = sql.Replace("WHERE", "");
            }
            if (!string.IsNullOrEmpty(groupBy))
            {
                sql = sql.Replace(";", string.Format(groupByTemplate, groupBy) + " ;");
            }

            if (!string.IsNullOrEmpty(having))
            {
                sql = sql.Replace(";", string.Format(havingTemplate, having) + " ;");
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                sql = sql.Replace(";", string.Format(orderByTemplate, orderBy) + " ;");
            }
            if (limit > 0)
            {
                sql = sql.Replace(";", string.Format(LimitTemplate, limit) + " ;");
            }

            if (offset > -1)
            {
                sql = sql.Replace(";", string.Format(offsetTemplate, offset) + " ;");
            }
            return sql;
        }

        /// <summary>
        ///拼装通用的插入SQL语句
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">类型的对像</param>
        /// <returns>插入SQL语句</returns>
        public static string GetInsertSql<T>(T obj)
        {
            string columNames = string.Empty;
            string values = string.Empty;
            Type type = typeof(T);
            PropertyInfo[] propertyinfos = type.GetProperties();
            object o;
            foreach (var property in propertyinfos)
            {
                o = property.GetValue(obj, null);
                if (o == null)
                {
                    continue;
                }
                if ( property.Name == "id" || property.Name == "Id")
                {
                    if((int)o <= 0) { continue; }                        
                }
                if (property.GetType() == typeof(DateTime) && Convert.ToDateTime(obj) < Convert.ToDateTime("1753-01-01"))
                {
                    continue;
                }
                if (columNames.Length == 0)
                {
                    columNames += splitChar + StringHelper.CamelCaseToDBnameing(property.Name) + splitChar;
                }
                else
                {
                    columNames += "," + splitChar + StringHelper.CamelCaseToDBnameing(property.Name) + splitChar;
                }
                if (values.Length == 0)
                {
                    values += valueSplitChar + property.GetValue(obj, null).ToString() + valueSplitChar;
                }
                else
                {
                    values += "," + valueSplitChar + o.ToString() + valueSplitChar;
                }
            }
            return string.Format(insertSqlTemplate, GetTableName(obj), columNames, values);
        }

        /// <summary>
        /// 获取修改SQL语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="set"></param>
        /// <param name="condition"></param>
        /// <returns>修改SQL语句 或 null </returns>
        public static string GetUpdateSql(string tableName, string set, string condition)
        {
            if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(set) || string.IsNullOrEmpty(condition))
            {
                return null;
            }
            return string.Format(updateSqlTemplate, tableName, set, condition);
        }


        /// <summary>
        ///拼装通用的修改SQL语句
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">类型的对像</param>
        /// <returns>修改SQL语句</returns>
        public static string GetUpdateSql<T>(T obj)
        {
            string set = string.Empty;
            string condition = string.Empty;
            Type type = typeof(T);
            PropertyInfo[] properinfors = type.GetProperties();
            object tempObj;
            foreach (PropertyInfo p in properinfors)
            {
                tempObj = p.GetValue(obj, null);
                if (tempObj == null)
                {
                    continue;
                }
                if (p.GetType() == typeof(DateTime) && Convert.ToDateTime(tempObj) < Convert.ToDateTime("1753-01-01"))
                {
                    continue;
                }
                if (p.Name == "id" || p.Name == "Id")
                {
                    if (tempObj != null && tempObj.ToString().Length > 0)
                    {
                        condition = splitChar + "id" + splitChar + " = " + valueSplitChar + tempObj.ToString() + valueSplitChar;
                    }
                }
                else
                {
                    if (set.Length == 0)
                    {
                        set = splitChar + StringHelper.CamelCaseToDBnameing(p.Name) + splitChar + " = " + valueSplitChar + tempObj.ToString() + valueSplitChar;
                    }
                    else
                    {
                        set += "," + splitChar + StringHelper.CamelCaseToDBnameing(p.Name) + splitChar + " = " + valueSplitChar + tempObj.ToString() + valueSplitChar;
                    }
                }

            }
            if (condition.Length == 0)
            {
                throw new Exception(buildSqlErrorMessage);
            }
            return string.Format(updateSqlTemplate, GetTableName(obj), set, condition);
        }

        /// <summary>
        ///拼装通用的删除SQL语句 判断是否支持软删除
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">类型的对像</param>
        ///   /// <param name="isTrueDelete">是否真的删除 默认软删除</param>
        /// <returns>删除SQL语句</returns>
        public static string GetDeleteSql<T>(T obj, bool isTrueDelete = true)
        {
            string condition = string.Empty;
            string id = string.Empty;
            Type type = typeof(T);
            try
            {
                PropertyInfo propertyinfo = type.GetProperty("id");
                if (propertyinfo == null)
                {
                    propertyinfo = type.GetProperty("Id");
                    if (propertyinfo == null)
                    {
                        throw new Exception(buildSqlErrorMessage);
                    }
                }
                object tempObj = propertyinfo.GetValue(obj, null);
                if (tempObj == null || tempObj.ToString().Length <= 0)
                {
                    throw new Exception(buildSqlErrorMessage);
                }
                if (isTrueDelete == false)
                {
                    try
                    {
                        PropertyInfo deletePropertyinfo = type.GetProperty(softDeletePropertyName);
                        if (deletePropertyinfo != null)
                        {
                            string where = splitChar + "id" + splitChar + "=" + valueSplitChar + tempObj.ToString() + valueSplitChar;
                            return GetUpdateSql(GetTableName(obj), softDeleteSet, where);
                        }
                    }
                    catch
                    {
                        //nothing to do 不做什么处理，因为数据库的表不包含“is_delete”字段， 不支持软删除
                    }
                }
                condition = splitChar + "id" + splitChar + "=" + valueSplitChar + tempObj.ToString() + valueSplitChar;
            }
            catch (AmbiguousMatchException e)
            {
                throw new Exception(buildSqlErrorMessage + e.Message);
            }
            catch (ArgumentNullException e)
            {
                throw new Exception(buildSqlErrorMessage + e.Message);
            }
            return string.Format(deleteSqlTemplate, GetTableName(obj), condition);
        }

        /// <summary>
        /// 获取删除SQL语句
        /// </summary>
        /// <param name="tableName">表名：不能为null</param>
        /// <param name="condition">条件：不能为null</param>
        /// <returns>SQL语句 或者 null</returns>
        public static string GetDeleteSql(string tableName, string condition)
        {
            if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(condition))
            {
                return null;
            }
            return string.Format(deleteSqlTemplate, tableName, condition);
        }
    }
}
