using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlDao
{
    /// <summary>
    /// String Helper
    /// </summary>
    public class StringHelper
    {

        /// <summary>
        /// 数据库命名法转化为驼峰命名法
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isBigCamelCaes">is Big Camel Caes</param>
        /// <returns></returns>
        public static string DBNamingToCamelCase(string name, bool isBigCamelCaes = false)
        {
            if (name == null || name.Length == 0) { return ""; }
            if (name.Contains("_"))
            {
                string[] words = name.Split('_');
                string result = string.Empty;
                for (int i = 0; i < words.Length; i++)
                {
                    if (i == 0)
                    {
                        result = words[i];
                    }
                    else
                    {
                        result += upperCaseFirstLetter(words[i]);
                    }
                }
                if (isBigCamelCaes == true)
                {
                    return upperCaseFirstLetter(result);
                }
                return result;
            }
            else
            {
                return name;
            }
        }
        /// <summary>
        /// 驼峰命名法转化为数据库命名法
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isBigCamelCaes">is Big Camel Caes</param>
        /// <returns></returns>
        public static string CamelCaseToDBnameing(string name, bool isBigCamelCaes = false)
        {
            if (name != null && name.Length > 0)
            {
                if (isBigCamelCaes == true)
                {
                    name = LowerCaseFirstLetter(name);
                }
                char[] array = name.ToCharArray();
                string result = string.Empty;
                for (int i = 0; i < array.Length; i++)
                {
                    if (i == 0)
                    {
                        result += array[i].ToString().ToLower();
                    }
                    else
                    {
                        if (isUpper(array[i]))
                        {
                            result += "_" + array[i].ToString().ToLower();
                        }
                        else if (IsInt(array[i].ToString()))
                        {
                            result += "_" + array[i].ToString();
                        }
                        else
                        {
                            result += array[i].ToString();
                        }
                    }
                }
                return result;
            }
            return "";
        }
        /// <summary>
        /// 是否为整型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsInt(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?/d*$");
        }
        /// <summary>
        /// Json 命名改为数据库命名
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string JsonCamelCaseToDBnameing(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            string resultString = string.Empty;
            string pattern = "([\"](\\w+?)[\"][:]{1}?)";
            MatchCollection colls = Regex.Matches(json, pattern);
            for (int i = 0; i < colls.Count; i++)
            {
                json = json.Replace(colls[i].ToString(), DBNamingToCamelCase(colls[i].ToString()));
            }
            return json;
        }
        /// <summary>
        /// Json 命名改为驼峰命名
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string JsonDBnameingToCamelCase(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            string resultString = string.Empty;
            string pattern = "([\"](\\w+?)[\"][:]{1}?)";
            MatchCollection colls = Regex.Matches(json, pattern);
            for (int i = 0; i < colls.Count; i++)
            {
                json = json.Replace(colls[i].ToString(), CamelCaseToDBnameing(colls[i].ToString()));
            }
            return json;
        }
        /// <summary>
        /// 数据库表名转化为类名
        /// </summary>
        /// <param name="dbname"> 数据库表名</param>
        /// <returns>类名</returns>
        public static string dbNameToClassName(string dbname)
        {
            return upperCaseFirstLetter(dbname);
        }


        /// <summary>
        /// 将一个单词的第一个字母变为大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string upperCaseFirstLetter(string str)
        {
            return str.Substring(0, 1).ToUpper() + str.Substring(1);
        }

        /// <summary>
        /// 将一个单词的第一个字母变为小写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string LowerCaseFirstLetter(string str)
        {
            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }
        /// <summary>
        /// 判断字符是否为大写字母
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool isUpper(char c)
        {
            return c > 'A' && c < 'Z';
        }
    }
}
