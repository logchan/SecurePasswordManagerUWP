using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace SecurePasswordManager.Core
{
    public static class UWPHelper
    {
        public static async Task ShowMessage(string title, string msg, double maxWidth)
        {
            var dialog = new ContentDialog();
            dialog.Title = title;
            dialog.MaxWidth = maxWidth;
            dialog.Content = msg;
            dialog.PrimaryButtonText = "OK";

            await dialog.ShowAsync();
        }

        public static async Task ShowMessage(this Page p, string title, string msg)
        {
            await ShowMessage(title, msg, p.ActualWidth);
        }

        public static async Task<bool> ShowYesNoMessage(string title, string msg, double maxWidth)
        {
            var dialog = new ContentDialog();
            dialog.Title = title;
            dialog.MaxWidth = maxWidth;
            dialog.Content = msg;
            dialog.PrimaryButtonText = "Yes";
            dialog.SecondaryButtonText = "No";

            bool clickedYes = false;
            dialog.PrimaryButtonClick += (o, e) => { clickedYes = true; };

            await dialog.ShowAsync();
            return clickedYes;
        }

        public static async Task<bool> ShowYesNoMessage(this Page p, string title, string msg)
        {
            return await ShowYesNoMessage(title, msg, p.ActualWidth);
        }

        public static async Task<string> PromptForInput(string title, string msg, string defaultValue, double maxWidth)
        {
            var dialog = new ContentDialog();
            dialog.Title = title;
            dialog.MaxWidth = maxWidth;

            var panel = new StackPanel();
            panel.Children.Add(new TextBlock { Text = msg, TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap });
            var textBox = new TextBox();
            textBox.Text = defaultValue;
            textBox.KeyUp += (o, e) =>
            {
                if (e.Key == Windows.System.VirtualKey.Enter)
                    dialog.Hide();
                e.Handled = true;
            };
            panel.Children.Add(textBox);

            dialog.Content = panel;
            dialog.PrimaryButtonText = "OK";

            await dialog.ShowAsync();
            return textBox.Text;
        }

        public static async Task<string> PromptForInput(this Page p, string title, string msg, string defaultValue)
        {
            return await PromptForInput(title, msg, defaultValue, p.ActualWidth);
        }

        public static async Task<string> PromptForSecret(string title, string msg, double maxWidth)
        {
            var dialog = new ContentDialog();
            dialog.Title = title;
            dialog.MaxWidth = maxWidth;

            var panel = new StackPanel();
            panel.Children.Add(new TextBlock { Text = msg, TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap });
            var passwordBox = new PasswordBox();
            passwordBox.IsPasswordRevealButtonEnabled = true;
            passwordBox.KeyUp += (o, e) =>
            {
                if (e.Key == Windows.System.VirtualKey.Enter)
                    dialog.Hide();
                e.Handled = true;
            };
            panel.Children.Add(passwordBox);

            dialog.Content = panel;
            dialog.PrimaryButtonText = "OK";

            await dialog.ShowAsync();
            return passwordBox.Password;
        }

        public static async Task<string> PromptForSecret(this Page p, string title, string msg)
        {
            return await PromptForSecret(title, msg, p.ActualWidth);
        }
    }
}
