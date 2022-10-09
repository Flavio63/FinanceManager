using System;
using System.Globalization;
using System.Windows.Data;

namespace FinanceManager.Events
{
    public class RowTotalStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                Console.WriteLine(value.ToString().ToLower() + parameter.ToString().ToLower());
                return value.ToString().ToLower().StartsWith(parameter.ToString().ToLower());
            }
            catch
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }    
}
