using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Charts
{
    [ValueConversion(typeof(double), typeof(string))]
    class String2DoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string numS = (string)value;
            if (numS.Length == 0)
                return 0;
            if (numS[numS.Length - 1] == ',')
                numS += "0";
            if (numS.StartsWith(","))
                numS = "0" + numS;
            return double.Parse(numS);
        
        }
    }
}
