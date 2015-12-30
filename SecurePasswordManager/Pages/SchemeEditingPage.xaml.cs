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
using SecurePasswordManager.Model.Scheme;
using SecurePasswordManager.Core;
using System.Threading.Tasks;

namespace SecurePasswordManager.Pages
{
    public sealed partial class SchemeEditingPage : Page
    {
        AppManager manager;
        ListExtraMemberHelper<SPMSchemeField> fieldsForList;
        SPMScheme currScheme;
        bool isNewScheme = false;

        public SchemeEditingPage()
        {
            manager = AppManager.GetInstance();
            this.InitializeComponent();

            foreach (var v in Enum.GetValues(typeof(SPMSchemeProcessType)))
            {
                procCombo.Items.Add(v);
            }
        }

        private void updateFieldsList()
        {
            fieldsList.ItemsSource = null;
            fieldsList.ItemsSource = fieldsForList;
        }

        private void updateIterFieldCombo()
        {
            int fnumbefore = iterFieldCombo.Items.Count;
            int selbefore = iterFieldCombo.SelectedIndex;

            iterFieldCombo.Items.Clear();
            foreach (var field in currScheme.Fields)
            {
                iterFieldCombo.Items.Add(field.Name);
            }
            int fnumnow = iterFieldCombo.Items.Count;
            if (fnumnow < fnumbefore)
                iterFieldCombo.SelectedIndex = 0;
            else
                iterFieldCombo.SelectedIndex = selbefore;
        }

        private async Task TryGoBack()
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
            else
            {
                await this.ShowMessage("Error", "Critical error occurred.");
                // TODO: log
                Application.Current.Exit();
            }
        }

        private async Task<bool> UpdateCurrSchemeExceptFields()
        {
            if (!SPMScheme.IsNameValid(nameBox.Text))
            {
                await this.ShowMessage("Invalid Name", "The name of scheme should have at most 32 characters of letters, numbers, and these characters: []().,_ But it shall not end with a dot or a space.");
                return false;
            }

            bool iterIsFixed = iterChoiceFixed.IsChecked ?? false;
            int param = 0;
            if (iterIsFixed)
            {
                if (!int.TryParse(numIterBox.Text, out param))
                {
                    await this.ShowMessage("Invalid Number", "The number of iteration is not valid.");
                    return false;
                }
                else
                {
                    if (param < 1 || param > 1000)
                    {
                        await this.ShowMessage("Iteration Out of Range", "The number of iteration should be in [1, 1000]");
                        return false;
                    }
                }
            }
            else
            {
                param = iterFieldCombo.SelectedIndex;
            }

            currScheme.Name = nameBox.Text;
            currScheme.Description = descBox.Text;
            currScheme.ProcessType = (SPMSchemeProcessType)procCombo.SelectedItem;
            
            currScheme.TimeToHashType = iterIsFixed ? SPMSchemeTimeToHashType.FIXED : SPMSchemeTimeToHashType.FROM_FIELD;
            currScheme.TimeToHashParam = param;
            return true;
        }

        private void sePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (manager.CurrentScheme == null)
            {
                currScheme = new SPMScheme();
                isNewScheme = true;
            }
            else
            {
                currScheme = new SPMScheme(manager.CurrentScheme);
            }

            if (currScheme.Fields.Count == 0)
            {
                currScheme.Fields.Add(new SPMSchemeField() { Name = "Default", Description = "Just fill in something.", SaltingType = SPMSchemeSaltingType.ALWAYS });
            }

            fieldsForList = new ListExtraMemberHelper<SPMSchemeField>();
            fieldsForList.members = currScheme.Fields;
            fieldsForList.membersAfter = new List<SPMSchemeField>()
            {
                new SPMSchemeField() { Name = "Add a new field", Description = "Tap here to add a new field", isActual = false }
            };
            updateFieldsList();

            foreach (var field in currScheme.Fields)
            {
                this.iterFieldCombo.Items.Add(field.Name);
            }

            int procIndex = CSHelper.IndexOfEnum(procCombo.Items, currScheme.ProcessType);
            this.procCombo.SelectedIndex = procIndex >= 0 ? procIndex : 0;

            if (currScheme.TimeToHashType == SPMSchemeTimeToHashType.FROM_FIELD)
            {
                this.iterChoiceLength.IsChecked = true;
                this.iterFieldCombo.SelectedIndex = currScheme.TimeToHashParam >= 0 && currScheme.TimeToHashParam < this.iterFieldCombo.Items.Count ? currScheme.TimeToHashParam : 0;
            }
            else
            {
                this.numIterBox.Text = currScheme.TimeToHashParam.ToString();
                this.iterFieldCombo.SelectedIndex = 0;
            }

            this.nameBox.Text = currScheme.Name;
            this.descBox.Text = currScheme.Description;
        }

        private async void fieldsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            SPMSchemeField field = (SPMSchemeField)e.ClickedItem;
            if (fieldsForList.membersAfter.FindIndex(f => ReferenceEquals(field, f)) < 0)
            {
                bool kept = await PromptFieldEdit(field);
                if (!kept)
                {
                    if (currScheme.Fields.Count > 1)
                    {
                        currScheme.Fields.Remove(field);
                    }
                    else
                    {
                        await this.ShowMessage("Error", "At least one field should be kept.");
                    }
                }
            }
            else
            {
                field = new SPMSchemeField();
                currScheme.Fields.Add(field);
                updateFieldsList();

                bool kept = await PromptFieldEdit(field);
                if (!kept)
                {
                    currScheme.Fields.Remove(field);
                }
            }

            updateFieldsList();
            updateIterFieldCombo();
        }

        private async Task<bool> PromptFieldEdit(SPMSchemeField field)
        {
            int result = 0;

            var dialog = new ContentDialog();
            dialog.Title = "Edit Field";
            dialog.MaxWidth = ActualWidth;

            var panel = new StackPanel();

            panel.Children.Add(new TextBlock { Text = "Name:", TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap });
            var nameBox = new TextBox();
            nameBox.Text = field.Name;
            panel.Children.Add(nameBox);

            panel.Children.Add(new TextBlock { Text = "Description:", TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap });
            var descBox = new TextBox();
            descBox.Text = field.Description;
            panel.Children.Add(descBox);

            panel.Children.Add(new TextBlock { Text = "Method:", TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap });
            var methCombo = new ComboBox();
            foreach(var v in Enum.GetValues(typeof(SPMSchemeSaltingType)))
            {
                methCombo.Items.Add(v);
            }
            int methIndex = CSHelper.IndexOfEnum(methCombo.Items, field.SaltingType);
            methCombo.SelectedIndex = methIndex >= 0 ? methIndex : 0;
            methCombo.HorizontalAlignment = HorizontalAlignment.Stretch;
            panel.Children.Add(methCombo);

            var delBtn = new Button();
            delBtn.Click += (o, e) => { result = 2; dialog.Hide(); };
            delBtn.Margin = new Thickness(0, 12, 0, 0);
            delBtn.Content = new TextBlock { Text = "Delete", Foreground = new SolidColorBrush(Windows.UI.Colors.Red), TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap };
            delBtn.HorizontalAlignment = HorizontalAlignment.Stretch;
            delBtn.HorizontalContentAlignment = HorizontalAlignment.Center;
            panel.Children.Add(delBtn);

            dialog.Content = panel;
            dialog.PrimaryButtonText = "OK";
            dialog.PrimaryButtonClick += (o, e) => { result = 1; };

            dialog.SecondaryButtonText = "Cancel";

#if DEBUG
            dialog.Tapped += (o, e) =>
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Height {0}, ActualHeight {1}, panel.Height {2}, panel.ActualHeight {3}", dialog.Height, dialog.ActualHeight, panel.Height,panel.ActualHeight));
            };
#endif

            await dialog.ShowAsync();
            
            if (result == 2) // delete
            {
                return false;
            }
            else if (result == 1) // save changes
            {
                field.Name = nameBox.Text;
                field.Description = descBox.Text;
                field.SaltingType = (SPMSchemeSaltingType)methCombo.SelectedItem;
            }
            return true;
        }

        private async void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            await TryGoBack();
        }

        private async void saveSchemeBtn_Click(object sender, RoutedEventArgs e)
        {
            bool update = await UpdateCurrSchemeExceptFields();
            if (!update)
                return;

            // check name collision
            var another = manager.Schemes.Find(s => string.Compare(currScheme.Name, s.Name, ignoreCase: true) == 0);
            if (another != null && (isNewScheme || !ReferenceEquals(another, manager.CurrentScheme)))
            {
                await this.ShowMessage("Name Collision", "There is already a scheme with the same name (not case-sensative). Please change the name of the scheme.");
                return;
            }

            bool result;
            if (isNewScheme)
            {
                manager.Schemes.Add(currScheme);
                manager.CurrentScheme = currScheme; // after inserting, references are the same (i.e. ReferenceEquals(currScheme, manager.Schemes[-1]))
                result = await manager.SaveCurrentSchemeAsync();
            }
            else
            {
                // updating scheme and field is troublesome
                // so just delete it and insert the current one
                bool delResult = await manager.DeleteCurrentSchemeAsync();
                if (!delResult)
                {
                    bool cont = await this.ShowYesNoMessage("Error", "An error occurred while trying to remove the old scheme. Continue saving?");
                    if (!cont)
                        return;
                }
                int oldIndex = manager.Schemes.IndexOf(manager.CurrentScheme);
                manager.Schemes.Insert(oldIndex + 1, currScheme);
                manager.Schemes.RemoveAt(oldIndex);
                manager.CurrentScheme = currScheme;
                result = await manager.SaveCurrentSchemeAsync();
            }

            if (!result)
            {
                // TODO: log
                bool giveup = await this.ShowYesNoMessage("Error", "Failed saving the scheme. Give up changes?");
                if (giveup)
                    await TryGoBack();
            }
            else
            {
                await TryGoBack();
            }
        }
    }
}
