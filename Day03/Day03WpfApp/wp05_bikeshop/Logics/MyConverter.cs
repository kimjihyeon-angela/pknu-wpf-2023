using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace wp05_bikeshop.Logics
{
    internal class MyConverter : IValueConverter
    {
        // 대상에 표현할 때 값을 변환해주는 경우 사용 (OnewWay)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() + " km/h";
        }

        // 대상값이 바껴 원본 소스의 값을 변환해서 표현해 줄 때 사용 (TwoWay)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return int.Parse(value.ToString()) * 3;
        }
    }
}
