using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDaoDemo
{
   public class User
    {
        public Int32 Id { get; set; }
        public String Name { get; set; }
        public Int32? Age { get; set; }
        public decimal? Salary { get; set; }
        public Int32? IsDelete { get; set; }
        public DateTime? DeleteTime { get; set; }
    }
}
