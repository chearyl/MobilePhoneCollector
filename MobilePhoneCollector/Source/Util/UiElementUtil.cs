using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobilePhoneCollector.Source.Util
{
    public class UiElementUtil
    {
        public static T? FindChildByName<T>(DependencyObject parent, string childName) where T : FrameworkElement
        {
            if (parent == null) return null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                // Check if the child matches the target type and Name
                if (child is T frameworkElement && frameworkElement.Name == childName)
                {
                    return frameworkElement;
                }

                // Recursively search deeper in the visual tree
                var foundChild = FindChildByName<T>(child, childName);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }
            return null;
        }
    }
}
