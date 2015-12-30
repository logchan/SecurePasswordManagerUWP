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

using SecurePasswordManager.SPMApp;

namespace SecurePasswordManager.Templates
{
    public sealed partial class AboutDialog : ContentDialog
    {
        private AppManager manager = AppManager.GetInstance();

        public AboutDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var settings = manager.CurrentSettings;
            if (settings != null)
            {
                versionText.Text = String.Format("Version {0}.{1}", settings.MajorVersion, settings.MinorVersion);
            }
        }
    }
}
