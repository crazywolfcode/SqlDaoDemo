using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDao
{
    public class MysqlTabeSchema : TableScema
    {
        public string Field { get; set; }
        public string Type { get; set; }
        public string Null { get; set; }
        public string Key { get; set; }
        public string Default { get; set; }
        public string Extra { get; set; }
    }
}
