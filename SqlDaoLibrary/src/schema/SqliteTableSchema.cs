using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDao
{
    public class SqliteTableSchema : TableScema
    {
        public string name { get; set; }
        public string type { get; set; }
        public string notnull { get; set; }
        public string dflt_value { get; set; }
        public string pk { get; set; }
    }
}
