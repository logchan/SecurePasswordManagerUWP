using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace SecurePasswordManager.Core
{
    class OpenMenuFlyoutAction : DependencyObject, IAction
    {

        public static readonly DependencyProperty ShallDisplayProperty = DependencyProperty.Register("ShallDisplay", typeof(bool), typeof(OpenMenuFlyoutAction), new PropertyMetadata(true));

        public bool ShallDisplay
        {
            get
            {
                return (bool)GetValue(ShallDisplayProperty);
            }

            set
            {
                SetValue(ShallDisplayProperty, value);
            }
        }

        public object Execute(object sender, object parameter)
        {
            if (!ShallDisplay)
                return null;

            FrameworkElement senderElement = sender as FrameworkElement;

            if (senderElement.Tag != null)
            {
                var type = senderElement.Tag.GetType();
                foreach (var prop in type.GetProperties())
                {
                    var attr = prop.GetCustomAttribute<MenuFlyoutIndicatorAttribute>(true);
                    if (attr != null && prop.PropertyType == typeof(bool))
                    {
                        bool noskip = (bool)prop.GetValue(senderElement.Tag);
                        if (!noskip)
                            return null;
                    }
                }
            }

            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);

            return null;
        }
    }
}
