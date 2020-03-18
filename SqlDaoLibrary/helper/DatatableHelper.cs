using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace SqlDao
{
    /// <summary>
    /// Datatable Helper
    /// </summary>
    public class DatatableHelper
    {

        /// <summary>  
        /// DataTable转化为List集合  
        /// </summary>  
        /// <typeparam name="T">实体对象</typeparam>  
        /// <param name="dataTable">datatable表</param>  
        /// <returns>返回list集合</returns>  
        public static List<T> DataTableToList<T>(DataTable dataTable)
        {
            List<T> list = new List<T>();
            Type type = typeof(T);
            List<string> colums = new List<string>();
            foreach (DataRow row in dataTable.Rows)
            {
                //集合属性数组  
                PropertyInfo[] propertyinfos = type.GetProperties();
                //新建对象实例
                T entity = Activator.CreateInstance<T>();
                foreach (PropertyInfo p in propertyinfos)
                {
                    if (!dataTable.Columns.Contains(p.Name) || row[p.Name] == null || row[p.Name] == DBNull.Value)
                    {
                        continue;
                    }
                    try
                    {
                        //类型强转，将table字段类型转为集合字段类型  
                        var obj = Convert.ChangeType(row[p.Name], p.PropertyType);
                        p.SetValue(entity, obj, null);
                    }
                    catch (Exception)
                    {

                    }
                }
                list.Add(entity);
            }
            return list;
        }

        /// <summary>  
        /// List集合转DataTable  
        /// </summary>  
        /// <typeparam name="T">实体类型</typeparam>  
        /// <param name="list">传入集合</param>  
        /// <returns>返回datatable结果</returns>
        public static DataTable ListToDataTable<T>(List<T> list)
        {
            DataTable dataTable = new DataTable();
            DataRow dataRow;
            Type type = typeof(T);
            PropertyInfo[] propertyinfos = type.GetProperties();
            foreach (var item in propertyinfos)
            {
                dataTable.Columns.Add(item.Name, item.PropertyType);
            }
            foreach (var item in list)
            {
                dataRow = dataTable.NewRow();
                foreach (var propertyInfo in propertyinfos)
                {
                    object obj = propertyInfo.GetValue(item, null);
                    if (obj == null)
                    {
                        continue;
                    }
                    if (propertyInfo.GetType() == typeof(DateTime) && Convert.ToDateTime(obj) < Convert.ToDateTime("1753-01-01"))
                    {
                        continue;
                    }
                    dataRow[propertyInfo.Name] = obj;
                }
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }



        /// <summary>  
        /// 提取DataTable某一行转为指定对象
        /// </summary>  
        /// <typeparam name="T">实体</typeparam>  
        /// <param name="dataTable">传入的表格</param>  
        /// <param name="rowIndex">table行索引，默认为第一行</param>  
        /// <returns>返回实体对象</returns> 
        public static T DataTableToEntity<T>(DataTable dataTable, int rowIndex=0)
        {
            Type type = typeof(T);
            T entity = Activator.CreateInstance<T>();
            if (dataTable == null)
            {
                return entity;
            }
            DataRow row = dataTable.Rows[rowIndex];
            PropertyInfo[] pArray = type.GetProperties();
            foreach (PropertyInfo p in pArray)
            {
                if (!dataTable.Columns.Contains(p.Name) || row[p.Name] == null || row[p.Name] == DBNull.Value)
                {
                    continue;
                }

                if (p.PropertyType == typeof(DateTime) && Convert.ToDateTime(row[p.Name]) < Convert.ToDateTime("1753-01-02"))
                {
                    continue;
                }
                try
                {
                    //类型强转，将table字段类型转为对象字段类型 
                    var obj = Convert.ChangeType(row[p.Name], p.PropertyType);
                    p.SetValue(entity, obj, null);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return entity;
        }
    }
}
