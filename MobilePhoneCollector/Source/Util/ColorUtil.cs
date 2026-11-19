using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace MobilePhoneCollector.Source.Util
{
    public class ColorUtil
    {
        public static SolidColorBrush HexToColorBrush(string hex)
        {
            hex = hex.TrimStart('#');
            if (hex.Length > 6) hex = hex.Substring(hex.Length - 6);

            int r = Convert.ToInt32(hex.Substring(0, 2), 16);
            int g = Convert.ToInt32(hex.Substring(2, 2), 16);
            int b = Convert.ToInt32(hex.Substring(4, 2), 16);
            return new SolidColorBrush(Color.FromArgb(255, (byte)r, (byte)g, (byte)b));
        }
    }
}
