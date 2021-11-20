using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Charts
{
    /// <summary>
    /// Логика взаимодействия для CustomCanvas.xaml
    /// </summary>
    public partial class CustomCanvas : UserControl
    {
        private StackPanel legend = new();
        public double Width { get => OuterCanvas.Width; set
            {
                OuterCanvas.Width = value;
                MainCanvas.Width = value - 150;
                Invalidate();
            }
        }
        public double Height { get => OuterCanvas.Height; set
            {
                OuterCanvas.Height = value;
                MainCanvas.Height = value- 30;
                Invalidate();
            }
        }
        public double Step { get => 0.1 / Scale; }
        public Point Center { get => new(MainCanvas.Width / 2, MainCanvas.Height / 2); }
        public Point ViewCenter = new(0, 0);
        double Scale = 1;
        public double ScaleX { get => 1 / (MainCanvas.Width/2); }
        public double ScaleY { get => - 1 / (MainCanvas.Height /2); }
        private List<IPointExporter> polylines = new();
        private List<double> gridX = new();
        private List<double> gridY = new();

        public CustomCanvas()
        {
            InitializeComponent();
            MainCanvas.ClipToBounds = true;

            for (int i = -10; i <= 10; i++)
            {
                gridX.Add(i / 10.0);
                gridY.Add(i / 10.0);
            }
        }
        
        public void Add(IPointExporter line)
        {
            IPointExporter pl = line.Copy();
            polylines.Add(line);
            line = viewProjection(line);
            foreach (Point th in line.Points)
            {
                pl.Points.Add(pointTransfrom(th));
            }

            MainCanvas.Children.Add(pl.Target);
            setLegend();
        }

        private Point pointTransfrom(Point point)
        {
            Vector vector = (Vector) point;
            vector.X /= ScaleX;
            vector.Y /= ScaleY;
            var t = (Vector)vector + (Vector)Center;
            return (Point) t;
        }

        public void Invalidate()
        {
            MainCanvas.Children.Clear();
            OuterCanvas.Children.Clear();
            OuterCanvas.Children.Add(MainCanvas);
            setLegend();
            invalidateLegend();
            List<IPointExporter> grid = invalidateGrid();
            var centered = viewProjection(polylines);
            grid.AddRange(centered);
            canvasProjection(grid);
            return;
        }

        private Polyline createPoly(Brush style, params Point[] points)
        {
            Polyline polyline = new();
            polyline.Stroke = style;
            foreach(var th in points)
                polyline.Points.Add(th);
            return polyline;
        }

        private List<IPointExporter> invalidateGrid()
        {
            List<IPointExporter> lines = new();
            foreach (double coord in gridX)
            {
                Polyline polyline = createPoly(Brushes.Gray, new(coord, -1), new(coord, 1));
                polyline.Opacity = 0.5;
                lines.Add(new PolylinePointExporter(polyline));
            }
            foreach (double coord in gridX)
            {
                Polyline polyline = createPoly(Brushes.Gray, new(-1, coord), new(1, coord));
                polyline.Opacity = 0.5;
                lines.Add(new PolylinePointExporter(polyline));
            }

            Polyline axisX = createPoly(Brushes.Black, new(ViewCenter.X * Scale, -1), new(ViewCenter.X * Scale, 1));
            Polyline axisY = createPoly(Brushes.Black, new(-1, ViewCenter.Y * Scale), new(1, ViewCenter.Y * Scale));
            lines.Add(new PolylinePointExporter(axisX));
            lines.Add(new PolylinePointExporter(axisY));
            return lines;
        }

        private void canvasProjection(List<IPointExporter> lines)
        {
            foreach (var line in lines)
            {
                IPointExporter poly = line.Copy();
                List<Point> points = new();
                foreach (Point th in line.Points)
                {
                    poly.Points.Add(pointTransfrom(th));
                }
                MainCanvas.Children.Add(poly.Target);
            }
        }

        private IPointExporter viewProjection(IPointExporter line)
        {
            IPointExporter poly = line.Copy();
            foreach (Point th in line.Points)
            {
                Point p = (Point)(((Vector)th  + (Vector)ViewCenter) * Scale);
                poly.Points.Add(p);
            }
            return poly;
        }

        private List<IPointExporter> viewProjection(List<IPointExporter> lines)
        {
            List<IPointExporter> rs = new();
            foreach (var line in lines)
            {
                rs.Add(viewProjection(line));
            }
            return rs;
        }

        private void invalidateLegend()
        {
            foreach (double coord in gridX)
            {
                TextBlock textBlock = new();
                textBlock.FontSize = 7;
                Point point = new(coord, -1);
                textBlock.Text = String.Format("{0:0.00}", (-ViewCenter.X + point.X / Scale) );
                point = pointTransfrom(point);
                Canvas.SetLeft(textBlock, point.X - 10);
                Canvas.SetTop(textBlock, point.Y);
                OuterCanvas.Children.Add(textBlock);
            }
            foreach (double coord in gridY)
            {
                TextBlock textBlock = new();
                textBlock.FontSize = 7;
                Point point = new(1, coord);
                textBlock.Text = String.Format("{0:0.00}", (-ViewCenter.Y + point.Y / Scale));
                point = pointTransfrom(point);
                Canvas.SetLeft(textBlock, point.X + 10);
                Canvas.SetTop(textBlock, point.Y);
                OuterCanvas.Children.Add(textBlock);
            }
        }
        private void viewCorrection()
        {
            ViewCenter.X = (int)(ViewCenter.X / Step) * Step;
            ViewCenter.Y = (int)(ViewCenter.Y / Step) * Step;
        }
        public void UpScale()
        {
            if (Scale > 16)
                return;
            Scale *= 2;
       
            Invalidate();
        }
        public void DownScale()
        {
            if (Scale < 0.125)
                return;
            Scale /= 2;
            viewCorrection();
            Invalidate();
        }

        public void Right()
        {
            ViewCenter.X += Step;
            Invalidate();
        }
        public void Left()
        {
            ViewCenter.X -= Step;
            Invalidate();
        }
        public void Down()
        {
            ViewCenter.Y -= Step;
            Invalidate();
        }
        public void Up()
        {
            ViewCenter.Y += Step;
            Invalidate();
        }

        public void setLegend()
        {
            OuterCanvas.Children.Remove(legend);
            legend.Children.Clear();
            foreach (var t in polylines)
            {
                StackPanel lstack = new();
                lstack.Orientation = Orientation.Horizontal;
                Line line = new Line();
                line.X1 = 0;
                line.X2 = 50;
                line.Y1 = 0;
                line.Y2 = 0;
                line.Stroke = new SolidColorBrush(t.Color);
                TextBlock text = new TextBlock();
                text.VerticalAlignment = VerticalAlignment.Center;
                text.Text = t.Name;
                lstack.Children.Add(line);
                lstack.Children.Add(text);
                lstack.VerticalAlignment = VerticalAlignment.Center;
                legend.Children.Add(lstack);
            }
            Canvas.SetLeft(legend, OuterCanvas.Width - 100);
            Canvas.SetTop(legend, 50);
            OuterCanvas.Children.Add(legend);
        }
    }
}
