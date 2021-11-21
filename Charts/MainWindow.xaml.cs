using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Charts
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, IPointExporter> name_to_shape = new();
        ObservableCollection<Table> tables = new();
        public MainWindow()
        {
            InitializeComponent();
            Canvas.Width = 580;
            Canvas.Height = 480;
            comboBox.ItemsSource = tables;

           // Canvas.Add(new PolylinePointExporter(line));
        }
        private void Down(object sender, RoutedEventArgs e) => Canvas.DownScale();
       
        private void Up(object sender, RoutedEventArgs e) => Canvas.UpScale();

        private void LeftClick(object sender, RoutedEventArgs e) => Canvas.Right();

        private void RightClick(object sender, RoutedEventArgs e) => Canvas.Left();
        
        private void TopClick(object sender, RoutedEventArgs e) => Canvas.Up();
        
        private void BottomClick(object sender, RoutedEventArgs e) => Canvas.Down();

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(e.Delta > 0)
            {
                Canvas.UpScale();
            }
            else
            {
                Canvas.DownScale();
            }
        }

        private void Canvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
                Canvas.Up();
            else if (e.Key == Key.Down)
                Canvas.Down();
            else if (e.Key == Key.Left)
                Canvas.Left();
            else if (e.Key == Key.Right)
                Canvas.Right();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog().Value)
            {
                Table table = (Table)comboBox.SelectedItem;
                File.WriteAllText(saveFileDialog.FileName, table.ToString());
            }

        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog().Value)
            {
                string[] file = File.ReadAllLines(openFileDialog.FileName);
                if (file.Length == 0)
                {
                    MessageBox.Show("Пустой файл!", "Файл не содержит данных", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                ObservableCollection<ObservablePoint> list = new();
                string tableName = openFileDialog.SafeFileName.Split(".")[0];
                for (int i = 1; i < file.Length; i++)
                {
                    string line = file[i];
                    string[] coords = line.Split(" ");
                    if (coords.Length != 2)
                    {
                        MessageBox.Show("Неверный формат файла!", "Файл не соответствует формату!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    try
                    {
                        ObservablePoint point = new(double.Parse(coords[0]), double.Parse(coords[1]));
  
                        list.Add(point);
                    }catch(Exception exp)
                    {
                        MessageBox.Show("Неверный формат файла!", "Файл не соответствует формату!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                
                Table table = new(file[0], tableName, list);
                tables.Add(table);
                Polyline polyline = new Polyline();
                IPointExporter exporter = new PolylinePointExporter(polyline);
                foreach (var th in list)
                {
                    th.PropertyChanged += (t, e) =>
                    {
                        PointCollection pointC = new();
                        foreach(var thp in list)
                        {
                            pointC.Add(thp.Point);
                        }
                        exporter.Points = pointC;
                        Canvas.Update();
                    };
                    polyline.Points.Add(th.Point);
                }
                polyline.Stroke = Brushes.Black;
                
                exporter.Name = tableName;
                name_to_shape.Add(tableName, exporter);
                Canvas.Add(exporter);
                Controls.Visibility = Visibility.Visible;
                comboBox.SelectedItem = table;
            }
        }

        private void comboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Table selectedItem = (Table)comboBox.SelectedItem;
            Grid.ItemsSource = selectedItem.TableItems;
            colorPicker.SelectedColor = name_to_shape[selectedItem.TableName].Color;

            MainWindowF.Focus();
        }

        private void colorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Table selectedItem = (Table)comboBox.SelectedItem;
            name_to_shape[selectedItem.TableName].Color = colorPicker.SelectedColor.Value;
            Canvas.Invalidate();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            Regex regex = new Regex("^-?[0-9\\,]+$");
            string newS = textBox.Text + e.Text;
            e.Handled = !regex.IsMatch(newS);
        }
        private Point mouseCoord;
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var t = (Vector)mouseCoord - (Vector)e.GetPosition(Canvas);
                t.X = -t.X;
                Canvas.ViewCenter = (Point)((Vector)Canvas.ViewCenter + t * Canvas.Step / 10);
                Trace.WriteLine("Down X: " + t.X);
                Trace.WriteLine("Down Y: " + t.Y);
                mouseCoord = e.GetPosition(Canvas);
                Canvas.Update();
            }
            else
            {
                mouseCoord = e.GetPosition(Canvas);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Table selectedItem = (Table)comboBox.SelectedItem;
            selectedItem.TableItems.Add(new ObservablePoint(0, 0));
            name_to_shape[selectedItem.TableName].Points.Add(new Point(0, 0));
            Canvas.Update();
        }

        private void CenterButton_Click(object sender, RoutedEventArgs e)
        {
            Canvas.ViewCenter = new(0, 0);
            Canvas.Scale = 1;
            Canvas.Update();
        }
    }
}
