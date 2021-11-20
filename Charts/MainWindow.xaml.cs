using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
                List<Point> list = new();
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
                        list.Add(new(double.Parse(coords[0]), double.Parse(coords[1])));
                    }catch(Exception exp)
                    {
                        MessageBox.Show("Неверный формат файла!", "Файл не соответствует формату!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                string tableName = openFileDialog.SafeFileName.Split(".")[0];
                Table table = new(tableName, list);
                tables.Add(table);
                Polyline polyline = new Polyline();
                foreach(var th in list)
                {
                    polyline.Points.Add(th);
                }
                polyline.Stroke = Brushes.Black;
                IPointExporter exporter = new PolylinePointExporter(polyline);
                exporter.Name = tableName;
                name_to_shape.Add(tableName, exporter);
                Canvas.Add(exporter);
      
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
    }
}
