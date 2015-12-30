using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using SecurePasswordManager.Core;
using SecurePasswordManager.SPMApp;
using SecurePasswordManager.Model.Hashing;
using SecurePasswordManager.Model.Scheme;
using System.Threading.Tasks;

namespace SecurePasswordManager.Pages
{
    public sealed partial class PasswordGeneratingPage : Page
    {
        private AppManager manager;
        private List<string> fieldValues;
        private string lastGeneratedPwd = "";
        private bool pwdShowing = false;

        public PasswordGeneratingPage()
        {
            manager = AppManager.GetInstance();
            this.InitializeComponent();
            overrideIterPanel.Visibility = Visibility.Collapsed;

            foreach (var v in Enum.GetValues(typeof(SPMSchemeProcessType)))
            {
                procCombo.Items.Add(v);
            }
        }

        private void SetupPwdText()
        {
            pwdText.Text = "Password Ready. Tap to view (tap again to hide).";
            pwdPanel.Visibility = Visibility.Visible;
            pwdShowing = false;
        }

        private async void fieldsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var field = e.ClickedItem as SPMSchemeField;
            int index = manager.CurrentScheme.Fields.FindIndex(f => ReferenceEquals(f, field));
            if (index > -1)
            {
                string value = await this.PromptForInput("Salt", string.Format("Input the salt value for {0}:", field.Name), fieldValues[index]);
                fieldValues[index] = value;
            }
            else
            {
                await this.ShowMessage("Error", "Critical error: invalid index.");
            }
        }

        private void overrideIterCheck_Checked(object sender, RoutedEventArgs e)
        {
            overrideIterPanel.Visibility = Visibility.Visible;
        }

        private void overrideIterCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            overrideIterPanel.Visibility = Visibility.Collapsed;
        }

        private async void generatePwdBtn_Click(object sender, RoutedEventArgs e)
        {
            bool ovd = overrideIterCheck.IsChecked ?? false;
            int numiter = 0;
            if (ovd)
            {
                bool fixedIter = iterChoiceFixed.IsChecked ?? false;
                if (fixedIter)
                {
                    if (!Int32.TryParse(numIterBox.Text, out numiter))
                    {
                        await this.ShowMessage("Error", "Invalid number of iterations. Check your input.");
                        return;
                    }
                }
                else
                {
                    int index = iterFieldCombo.SelectedIndex;
                    numiter = fieldValues[index].Length;
                }
            }
            else
            {
                switch (manager.CurrentScheme.TimeToHashType)
                {
                    case SPMSchemeTimeToHashType.FIXED:
                        numiter = manager.CurrentScheme.TimeToHashParam;
                        break;
                    case SPMSchemeTimeToHashType.FROM_FIELD:
                        int index = manager.CurrentScheme.TimeToHashParam;
                        if (index < this.fieldValues.Count)
                        {
                            numiter = fieldValues[index].Length;
                        }
                        else
                        {
                            numiter = 0;
                        }
                        break;
                }
            }

            if (numiter < 1 || numiter > 1000)
            {
                await this.ShowMessage("Error", "Number of iterations out of range: [1, 1000].");
                return;
            }

            PasswordGenerator gen = HashingFactory.GetPwdGenerator(manager.CurrentScheme, fieldValues, true, (SPMSchemeProcessType)procCombo.SelectedItem);
            string secret = await this.PromptForSecret("Secret", "Make sure no one else is looking and type your secret:");
            lastGeneratedPwd = gen.GeneratePassword(secret, numiter);

            SetupPwdText();
        }

        private async void pgPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (manager.CurrentScheme != null)
            {
                this.fieldsList.ItemsSource = manager.CurrentScheme.Fields;
                fieldValues = new List<string>(manager.CurrentScheme.Fields.Count);
                for (int i = 0; i < manager.CurrentScheme.Fields.Count; ++i)
                {
                    fieldValues.Add("");
                }

                switch (manager.CurrentScheme.TimeToHashType)
                {
                    case SPMSchemeTimeToHashType.FIXED:
                        this.iterationText.Text = string.Format("Fixed: {0} times.", manager.CurrentScheme.TimeToHashParam);
                        break;
                    case SPMSchemeTimeToHashType.FROM_FIELD:
                        int index = manager.CurrentScheme.TimeToHashParam;
                        if (index < this.fieldValues.Count)
                        {
                            this.iterationText.Text = string.Format("Length of {0}.", manager.CurrentScheme.Fields[index].Name);
                        }
                        else
                        {
                            this.iterationText.Text = string.Format("Length (Param invalid). Please override.");
                        }
                        break;
                }

                foreach (var field in manager.CurrentScheme.Fields)
                {
                    this.iterFieldCombo.Items.Add(field.Name);
                }
                this.iterFieldCombo.SelectedIndex = 0;

                int procIndex = CSHelper.IndexOfEnum(this.procCombo.Items, manager.CurrentScheme.ProcessType);
                this.procCombo.SelectedIndex = procIndex >= 0 ? procIndex : 0;
            }
            else
            {
                await this.ShowMessage("Error", "Critical error: null scheme passed to the page.");
                // TODO: log

                if (Frame.CanGoBack)
                {
                    Frame.GoBack();
                }
                else
                {
                    Frame.Navigate(typeof(MainPage));
                }
            }
        }

        private void pwdText_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (pwdShowing)
            {
                SetupPwdText();
            }
            else
            {
                pwdText.Text = lastGeneratedPwd;
                pwdShowing = true;
            }
        }

        private async Task CopyGeneratedPwd(int begin, int length)
        {
            if (begin < 0)
                begin = 0;

            if (begin + length > lastGeneratedPwd.Length)
                length = lastGeneratedPwd.Length - begin;

            if (length < 0)
                length = 0;

            DataPackage package = new DataPackage();
            if (length > 0)
            {
                package.SetText(lastGeneratedPwd.Substring(begin, length));
            }
            else
            {
                package.SetText("");
            }

            Clipboard.SetContent(package);
            await this.ShowMessage("Password Copied", String.Format("Successfully copied the password of length {0}", length));
        }

        private async void MenuFlyoutCopy_Click(object sender, RoutedEventArgs e)
        {
            string tag = (string)((MenuFlyoutItem)sender).Tag;
            int mode = 0;
            Int32.TryParse(tag, out mode);

            switch(mode)
            {
                case 0:
                    // copy all
                    await CopyGeneratedPwd(0, lastGeneratedPwd.Length);
                    break;
                case 1:
                    // copy first 16
                    await CopyGeneratedPwd(0, 16);
                    break;
            }
        }
    }
}
