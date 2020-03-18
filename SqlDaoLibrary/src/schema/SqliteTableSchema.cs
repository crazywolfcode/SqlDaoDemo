using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDao
{
    public class SqliteTableSchema : TableScema
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Notnull { get; set; }
        public string Dflt_value { get; set; }
        public string Pk { get; set; }
    }
}
