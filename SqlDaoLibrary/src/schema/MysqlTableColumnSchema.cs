using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDao
{
   public class MysqlTableColumnSchema
    {
        public string ColumnName { get; set; }
        public string Type { get; set; }
        public string DefaultValue { get; set; }
        public string IsNullable { get; set; }
        public string CommentValue { get; set; }
    }
}
