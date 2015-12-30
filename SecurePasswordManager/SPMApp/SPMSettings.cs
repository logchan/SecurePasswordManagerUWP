using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Windows.ApplicationModel;

using SecurePasswordManager.Core;

namespace SecurePasswordManager.SPMApp
{
    [XmlRoot("SPMSettings", IsNullable = false)]
    public class SPMSettings
    {
        private bool isFirstRun;

        public bool IsFirstRun
        {
            get { return isFirstRun; }
            set { isFirstRun = value; }
        }

        private int majorVersion;

        public int MajorVersion
        {
            get { return majorVersion; }
            set { majorVersion = value; }
        }

        private int minorVersion;

        public int MinorVersion
        {
            get { return minorVersion; }
            set { minorVersion = value; }
        }

        public SPMSettings()
        {
            PackageVersion version = Package.Current.Id.Version;
            isFirstRun = true;
            majorVersion = version.Major;
            minorVersion = version.Minor;
        }

        public static SPMSettings DeserializeXml(string xmlData)
        {
            try
            {
                return SerializationHelper.DeserializeXml<SPMSettings>(xmlData);
            }
            catch (Exception e)
            {
                // TODO: log
                return null;
            }
        }

        public static string SerializeXml(SPMSettings s)
        {
            try
            {
                return SerializationHelper.SerializeXml(s);
            }
            catch (Exception e)
            {
                // TODO: log
                return null;
            }
        }
    }
}
