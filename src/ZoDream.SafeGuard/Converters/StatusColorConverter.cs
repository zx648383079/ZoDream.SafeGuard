using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using ZoDream.Shared.Models;

namespace ZoDream.SafeGuard.Converters
{
    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FileTransformStatus o)
            {
                return o switch
                {
                    FileTransformStatus.Waiting or FileTransformStatus.Doing => new SolidColorBrush(Colors.Gray),
                    FileTransformStatus.Done => new SolidColorBrush(Colors.Green),
                    _ => new SolidColorBrush(Colors.Black)
                };
            } else if (value is FileCheckStatus c)
            {
                return c switch
                {
                    FileCheckStatus.Waiting or FileCheckStatus.Checking => new SolidColorBrush(Colors.Gray),
                    FileCheckStatus.Normal or FileCheckStatus.Valid => new SolidColorBrush(Colors.Green),
                    FileCheckStatus.Poisoning => new SolidColorBrush(Colors.Yellow),
                    FileCheckStatus.Virus => new SolidColorBrush(Colors.Red),
                    _ => new SolidColorBrush(Colors.Black)
                };
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
