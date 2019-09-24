using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDao
{
    public class JsonHelper
    {
        /// <summary>
        /// 从一个对象信息生成Json串 包含DataTable
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjectToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        /// <summary>
        /// 从一个Json串生成对象信息
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object JsonToObject(string jsonString, Type type)
        {
            return JsonConvert.DeserializeObject(jsonString, type);
        }
        /// <summary>
        /// 从一个Json串生成DataTable对象信息
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DataTable JsonToDataTable(string jsonString)
        {
            return JsonConvert.DeserializeObject<DataTable>(jsonString);
        }
    }
}
