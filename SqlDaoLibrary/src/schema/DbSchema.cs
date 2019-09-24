using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDao
{
    public class DbSchema
    {
        public string TableName { get; set; }
        public string TableComment { get; set; }
        public string CreateTime { get; set; }
        public string UpdateTime { get; set; }
        public string TableRows { get; set; }
        public string DataLength { get; set; }
    }
}
