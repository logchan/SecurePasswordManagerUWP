using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SecurePasswordManager.Templates
{
    public sealed partial class MasterGrid : UserControl
    {
        public MasterGrid()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty MainContentProperty = DependencyProperty.Register("MainContent", typeof(object), typeof(MasterGrid), new PropertyMetadata(null));

        public object MainContent
        {
            get
            {
                return GetValue(MainContentProperty);
            }

            set
            {
                SetValue(MainContentProperty, value);
            }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(MasterGrid), new PropertyMetadata(null));

        public object Title
        {
            get
            {
                return GetValue(TitleProperty);
            }

            set
            {
                SetValue(TitleProperty, value);
            }
        }

        private async void MenuFlyoutAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutDialog dialog = new AboutDialog() { MaxWidth = ActualWidth };
            await dialog.ShowAsync();
        }
    }
}
