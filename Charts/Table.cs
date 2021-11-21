using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<ObservablePoint> TableItems { get; set; }
        public Table(string Type, string TableName, ObservableCollection<ObservablePoint> TableItems)
        {
            this.Type = Type;
            this.TableItems = TableItems;
            this.TableName = TableName;
        }
        public override string ToString()
        {
            StringBuilder strb = new StringBuilder();
            strb.AppendLine(Type);
            foreach(var th in TableItems)
            {
                strb.AppendLine($"{th.X} {th.Y}");
            }
            return strb.ToString();
        }
    }
}
