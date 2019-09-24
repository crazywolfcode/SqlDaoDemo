using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDaoDemo
{
   public class Record
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public decimal? Money { get; set; }
        public string Remark { get; set; }
    }
}
