using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Charts
{
    public class PolylinePointExporter : IPointExporter
    {
        private Polyline polyline;
        public string Name { get; set; }
        public PolylinePointExporter() => this.polyline = new();
        
        public PolylinePointExporter(Polyline polyline) => this.polyline = polyline;
        
        public PointCollection Points { get => polyline.Points; set => polyline.Points = value; }

        public UIElement Target => this.polyline;

        public Color Color { get => ((SolidColorBrush)polyline.Stroke).Color; set => polyline.Stroke = new SolidColorBrush(value); }

        public IPointExporter Copy()
        {
            Polyline polylineC = new();
            polylineC.Stroke = polyline.Stroke;
            polylineC.Opacity = polyline.Opacity;
            return new PolylinePointExporter(polylineC);
        }

        public IPointExporter NewInstance() => new PolylinePointExporter();
    

        public void SetPoints(PointCollection points)
        {
            polyline.Points = points;
        }
    }
}
