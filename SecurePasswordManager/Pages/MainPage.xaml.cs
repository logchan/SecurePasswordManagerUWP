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

using SecurePasswordManager.Templates;
using SecurePasswordManager.Model.Scheme;
using SecurePasswordManager.SPMApp;
using SecurePasswordManager.Core;

namespace SecurePasswordManager.Pages
{
    public sealed partial class MainPage : Page
    {
        AppManager manager;
        ListExtraMemberHelper<SPMScheme> schemesForList;

        public MainPage()
        {
            manager = AppManager.GetInstance();
            this.InitializeComponent();

            schemesForList = new ListExtraMemberHelper<SPMScheme>();
            schemesForList.membersAfter = new List<SPMScheme>()
            {
                new SPMScheme { Name = "Add A New Scheme", Description = "Tap here to add a new scheme.", isActual = false }
            };
        }

        private void updateSchemesList()
        {
            schemesList.ItemsSource = null;
            schemesList.ItemsSource = schemesForList;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!manager.DataLoaded)
                await manager.LoadAllDataAsync();

            if (manager.Schemes != null)
            {
                schemesForList.members = manager.Schemes;
                updateSchemesList();
            }

            await manager.LoadSettings();
            if (manager.CurrentSettings == null)
            {
                await this.ShowMessage("Error", "Cannot load application settings.");
            }
            else
            {
                if (manager.CurrentSettings.IsFirstRun == true)
                {
                    await (new FirstRunDialog() { MaxWidth = ActualWidth }).ShowAsync();
                    manager.CurrentSettings.IsFirstRun = false;
                }
            }

            if (!(await manager.SaveCurrentSettings()))
            {
                await this.ShowMessage("Error", "Cannot save application settings.");
            }
        }

        private async void schemesList_ItemClick(object sender, ItemClickEventArgs e)
        {
            SPMScheme scheme = (SPMScheme)e.ClickedItem;
            if (schemesForList.membersAfter.FindIndex(s => ReferenceEquals(scheme, s)) < 0)
            {
                manager.CurrentScheme = scheme;
                Frame.Navigate(typeof(PasswordGeneratingPage));
            }
            else
            {
                manager.CurrentScheme = null;
                Frame.Navigate(typeof(SchemeEditingPage));
            }
        }

        private async void MenuFlyoutEdit_Click(object sender, RoutedEventArgs e)
        {
            SPMScheme scheme = (SPMScheme)((MenuFlyoutItem)sender).Tag;
            manager.CurrentScheme = scheme;
            Frame.Navigate(typeof(SchemeEditingPage));
        }

        private async void MenuFlyoutDelete_Click(object sender, RoutedEventArgs e)
        {
            SPMScheme scheme = (SPMScheme)((MenuFlyoutItem)sender).Tag;
            manager.CurrentScheme = scheme;

            bool sure = await this.ShowYesNoMessage(String.Format("Delete \"{0}\"", scheme.Name), "Are you sure?");
            if (sure)
            {
                bool result = await manager.DeleteCurrentSchemeAsync();
                if (!result)
                {
                    await this.ShowMessage("Error", "An error occurred while deleting. Sorry.");
                    return;
                }

                manager.Schemes.Remove(manager.CurrentScheme);
                manager.CurrentScheme = null;
                updateSchemesList();
            }
        }

        private async void MenuFlyoutDuplicate_Click(object sender, RoutedEventArgs e)
        {
            SPMScheme scheme = (SPMScheme)((MenuFlyoutItem)sender).Tag;
            manager.CurrentScheme = scheme;

            // get a name
            string n = null;
            bool prompting = true;
            while (prompting)
            {
                n = await this.PromptForInput("Duplicate Scheme", "Give the new scheme a name:", scheme.Name);

                if (!SPMScheme.IsNameValid(n))
                {
                    await this.ShowMessage("Invalid Name", "The name of scheme should have at most 32 characters of letters, numbers, and these characters: []().,_ But it shall not end with a dot or a space.");
                    continue;
                }

                if (manager.Schemes.Find(sc => String.Compare(n, sc.Name) == 0) == null)
                {
                    prompting = false;
                }
                else
                {
                    bool cont = await this.ShowYesNoMessage("Name Collision", "There is already a scheme with the same name (not case-sensative). Please change the name of the scheme. Tap Yes to try again, No to give up.");
                    if (!cont)
                        return;
                }
            }

            SPMScheme s = new SPMScheme(manager.CurrentScheme);
            s.Name = n;
            manager.Schemes.Insert(manager.Schemes.IndexOf(manager.CurrentScheme) + 1, s);
            manager.CurrentScheme = s;
            bool result = await manager.SaveCurrentSchemeAsync();
            if (!result)
            {
                await this.ShowMessage("Error", "An error occurred while writing the new scheme. Sorry.");
                manager.CurrentScheme = null;
                manager.Schemes.Remove(s);
                return;
            }

            updateSchemesList();
        }
    }
}
