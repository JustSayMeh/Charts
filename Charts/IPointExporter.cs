using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Charts
{
    public interface IPointExporter
    {
        string Name { get; set; }
        Color Color { get; set; }
        IPointExporter NewInstance();
        IPointExporter Copy();
        PointCollection Points { get; set; }
        void SetPoints(PointCollection points);
        UIElement Target {get;}
        
    }
}
