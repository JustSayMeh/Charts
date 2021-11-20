using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Charts
{
    class Table
    {
        public string Type { get; set; }
        public string TableName { get; set;}
        public List<Point> TableItems { get; set; }
        public Table(string TableName, List<Point> TableItems)
        {
            this.TableItems = TableItems;
            this.TableName = TableName;
        }
    }
}
