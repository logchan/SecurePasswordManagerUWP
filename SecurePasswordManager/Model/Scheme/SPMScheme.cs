using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

using SecurePasswordManager.Core;
using System.Text.RegularExpressions;

namespace SecurePasswordManager.Model.Scheme
{
    [XmlRoot("SPMScheme", IsNullable = false)]
    public class SPMScheme
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private SPMSchemeCrypto crypto;

        public SPMSchemeCrypto Crypto
        {
            get { return crypto; }
            set { crypto = value; }
        }

        private SPMSchemeTimeToHashType timeToHashType;

        public SPMSchemeTimeToHashType TimeToHashType
        {
            get { return timeToHashType; }
            set { timeToHashType = value; }
        }

        private int timeToHashParam;

        public int TimeToHashParam
        {
            get { return timeToHashParam; }
            set { timeToHashParam = value; }
        }

        private SPMSchemeProcessType processType;

        public SPMSchemeProcessType ProcessType
        {
            get { return processType; }
            set { processType = value; }
        }

        private List<SPMSchemeField> fields;

        public List<SPMSchemeField> Fields
        {
            get { return fields; }
        }

        public SPMScheme()
        {
            fields = new List<SPMSchemeField>();
            name = "New Scheme";
            description = "No description.";
            crypto = SPMSchemeCrypto.MD5;
            timeToHashType = SPMSchemeTimeToHashType.FIXED;
            timeToHashParam = 1;
            processType = SPMSchemeProcessType.NO_EFFECT;
        }

        public SPMScheme(SPMScheme o)
        {
            fields = new List<SPMSchemeField>(o.fields.Count);
            foreach(var field in o.fields)
            {
                fields.Add(new SPMSchemeField(field));
            }
            name = o.name;
            description = o.description;
            crypto = o.crypto;
            timeToHashType = o.timeToHashType;
            timeToHashParam = o.timeToHashParam;
            processType = o.processType;
        }

        public static bool IsNameValid(string name)
        {
            if (name.Length == 0 || name.Length > 32)
                return false;
            Regex r = new Regex(@"^[a-zA-Z0-9\-\._\s\[\]\(\),']+$");
            return r.IsMatch(name) && (!name.EndsWith(".") && !name.EndsWith(" "));
        }

        public static SPMScheme DeserializeXml(string xmlData)
        {
            try
            {
                return SerializationHelper.DeserializeXml<SPMScheme>(xmlData);
            }
            catch (Exception e)
            {
                // TODO: log
                return null;
            }
        }

        public static string SerializeXml(SPMScheme scheme)
        {
            try
            {
                return SerializationHelper.SerializeXml(scheme);
            }
            catch (Exception e)
            {
                // TODO: log
                return null;
            }
        }

        [XmlIgnore]
        public bool isActual = true;

        [XmlIgnore, MenuFlyoutIndicator]
        public bool IsActual
        {
            get
            {
                return isActual;
            }

            set
            {
                isActual = value;
            }
        }
    }
}
