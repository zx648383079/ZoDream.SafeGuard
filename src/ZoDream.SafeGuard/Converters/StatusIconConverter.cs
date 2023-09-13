using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using ZoDream.SafeGuard.Models;

namespace ZoDream.SafeGuard.Converters
{
    public class StatusIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FileTransformStatus o)
            {
                return o switch
                {
                    FileTransformStatus.Waiting => "\uE916",
                    FileTransformStatus.Doing => "\uE712",
                    FileTransformStatus.Done => "\uE930",
                    _ => string.Empty
                };
            }
            else if (value is FileCheckStatus c)
            {
                return c switch
                {
                    FileCheckStatus.Waiting => "\uE916",
                    FileCheckStatus.Checking => "\uE712",
                    FileCheckStatus.Normal => "\uE930",
                    FileCheckStatus.Poisoning => "\uE7BA",
                    FileCheckStatus.Virus => "\uE945",
                    _ => string.Empty
                };
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
