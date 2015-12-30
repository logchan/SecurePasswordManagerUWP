using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SecurePasswordManager.Model.Scheme;
using Windows.Storage;

namespace SecurePasswordManager.SPMApp
{
    class AppDataUtilities
    {
        private static async Task<IStorageFolder> OpenSchemeFolderAsync()
        {
            return await ApplicationData.Current.LocalFolder.CreateFolderAsync("schemes", CreationCollisionOption.OpenIfExists);
        }

        public static async Task<List<SPMScheme>> ReadAllSchemesAsync()
        {
            List<SPMScheme> result = new List<SPMScheme>();

            try
            {
                var folder = await OpenSchemeFolderAsync();
                var files = await folder.GetFilesAsync();
                foreach (var file in files)
                {
                    if (file.Name.EndsWith(".xml"))
                    {
                        string content = await FileIO.ReadTextAsync(file);
                        SPMScheme scheme = SPMScheme.DeserializeXml(content);
                        if (scheme != null)
                            result.Add(scheme);
                    }
                }
            }
            catch (Exception)
            {
                // TODO: add log
                return null;
            }

            return result;
        }

        public static async Task<bool> WriteSchemeAsync(SPMScheme scheme)
        {
            if (scheme == null)
                return true;

            try
            {
                var folder = await OpenSchemeFolderAsync();
                var file = await folder.CreateFileAsync(scheme.Name + ".xml", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, SPMScheme.SerializeXml(scheme));
            }
            catch (Exception)
            {
                // TODO: add log
                return false;
            }

            return true;
        }

        public static async Task<bool> WriteSchemesAsync(List<SPMScheme> schemes)
        {
            if (schemes == null)
                return true;

            try
            {
                var folder = await OpenSchemeFolderAsync();
                foreach (var scheme in schemes)
                {
                    var file = await folder.CreateFileAsync(scheme.Name + ".xml", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(file, SPMScheme.SerializeXml(scheme));
                }
            }
            catch (Exception)
            {
                // TODO: add log
                return false;
            }

            return true;
        }

        public static async Task<bool> DeleteSchemeAsync(string name)
        {
            try
            {
                var folder = await OpenSchemeFolderAsync();
                var file = await folder.GetFileAsync(name + ".xml");
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            catch (Exception)
            {
                // TODO: add log
                return false;
            }

            return true;
        }

        public static async Task<bool> DeleteAllSchemesAsync()
        {
            try
            {
                var folder = await OpenSchemeFolderAsync();
                var files = await folder.GetFilesAsync();
                foreach (var file in files)
                {
                    await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
            }
            catch (Exception)
            {
                // TODO: add log
                return false;
            }

            return true;
        }

        public static async Task<bool> RefreshSchemesAsync(List<SPMScheme> schemes)
        {
            return (await DeleteAllSchemesAsync()) && (await WriteSchemesAsync(schemes));
        }
    }
}
