using System;
using System.Globalization;
using System.Windows.Data;

namespace UserAdministration.Helpers
{
    /// <summary>
    /// Convert information that comming as MultiBindiing parameter and return it to ViewModel
    /// </summary>
    public class Converter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
