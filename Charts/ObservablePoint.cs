using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Charts
{
    class ObservablePoint : INotifyPropertyChanged
    {
        private Point point;
        public Point Point { get => point; }
        public event PropertyChangedEventHandler PropertyChanged;


        public double X { get => point.X; 
            set
            {
                point.X = value;
                OnPropertyChanged();
            }
        }

        public double Y
        {
            get => point.Y;
            set
            {
                point.Y = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged(string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public ObservablePoint(double X, double Y)
        {
            point = new(X, Y);
        }
    }
}
