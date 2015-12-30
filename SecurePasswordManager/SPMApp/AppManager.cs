using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SecurePasswordManager.Model.Scheme;
using Windows.Storage;
using System.IO;

namespace SecurePasswordManager.SPMApp
{
    class AppManager
    {
        private static AppManager instance = null;
        private static string SETTINGS_FILE = "spm_uwp_settings.xml";

        private List<SPMScheme> schemes = null;
        private bool dataLoaded = false;
        private SPMSettings settings = null;

        private AppManager()
        {

        }

        public static AppManager GetInstance()
        {
            if (instance == null)
                instance = new AppManager();
            return instance;
        }

        public async Task LoadSettings()
        {
            // determine whether a settings file already exists
            // if not, create it
            bool exists = false;
            StorageFile file = null;
            try
            {
                file = await ApplicationData.Current.LocalFolder.GetFileAsync(SETTINGS_FILE);
                exists = true;
            }
            catch (FileNotFoundException)
            {

            }
            catch (Exception e)
            {
                // TODO: log
            }

            // read the settings, or create a default one
            if (exists)
            {
                string content = await FileIO.ReadTextAsync(file);
                settings = SPMSettings.DeserializeXml(content);
                if (settings == null)
                {
                    // TODO: log
                    settings = new SPMSettings();
                }
            }
            else
                settings = new SPMSettings();
        }

        public async Task<bool> SaveCurrentSettings()
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(SETTINGS_FILE, CreationCollisionOption.ReplaceExisting);
                string content = SPMSettings.SerializeXml(settings);
                await FileIO.WriteTextAsync(file, content);
            }
            catch (Exception e)
            {
                // TODO: log
                return false;
            }

            return true;
        }

        public async Task LoadAllDataAsync()
        {
            schemes = await AppDataUtilities.ReadAllSchemesAsync();
        }

        public async Task<bool> RefreshSchemesAsync()
        {
            return await AppDataUtilities.RefreshSchemesAsync(schemes);
        }

        public async Task<bool> SaveCurrentSchemeAsync()
        {
            if (CurrentScheme == null)
                return false;

            return await AppDataUtilities.WriteSchemeAsync(CurrentScheme);
        }

        public async Task<bool> DeleteCurrentSchemeAsync()
        {
            if (CurrentScheme == null)
                return false;

            return await AppDataUtilities.DeleteSchemeAsync(CurrentScheme.Name);
        }

        public List<SPMScheme> Schemes
        {
            get
            {
                return schemes;
            }
        }

        public bool DataLoaded
        {
            get
            {
                return dataLoaded;
            }
        }

        public SPMScheme CurrentScheme
        {
            get;
            set;
        }

        public SPMSettings CurrentSettings
        {
            get
            {
                return settings;
            }
        }
    }
}
